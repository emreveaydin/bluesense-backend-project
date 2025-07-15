using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Domain.Contracts;
using MediatR;

namespace Core.Application.Features.Messages.Queries.SearchMessagesInGroup;

public class SearchMessagesInGroupQueryHandler : IRequestHandler<SearchMessagesInGroupQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public SearchMessagesInGroupQueryHandler(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MessageDto>> Handle(SearchMessagesInGroupQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.SearchMessagesInGroupAsync(request.GroupId, request.SearchText, request.PageNumber, request.PageSize);

        if (!messages.Any())
        {
            return Enumerable.Empty<MessageDto>();
        }
        
        var senderIds = messages.Select(m => m.SenderId).Distinct().ToList();
        var senders = (await _userRepository.FindAsync(u => senderIds.Contains(u.Id)))
            .ToDictionary(u => u.Id);

        var messageDtos = messages.Select(message =>
        {
            var senderDto = senders.TryGetValue(message.SenderId, out var sender)
                ? new UserDto(sender.Id, sender.Username, sender.Email)
                : new UserDto(message.SenderId, "Unknown", "Unknown");

            return new MessageDto(
                message.Id,
                message.Content,
                senderDto,
                message.GroupId,
                message.CreatedAt,
                message.IsEdited,
                message.FileUrl
            );
        }).ToList();

        return messageDtos;
    }
} 