using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Domain.Contracts;
using MediatR;

namespace Core.Application.Features.Messages.Queries.GetMessagesInGroup;

public class GetMessagesInGroupQueryHandler : IRequestHandler<GetMessagesInGroupQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public GetMessagesInGroupQueryHandler(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesInGroupQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetMessagesForGroupAsync(request.GroupId, request.PageNumber, request.PageSize);
        
        // This is a simplified approach. In a high-performance scenario,
        // this would cause an N+1 query problem.
        // A better approach would be to get all sender IDs, query users once, and map them.
        var messageDtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var sender = await _userRepository.GetByIdAsync(message.SenderId);
            var senderDto = sender != null 
                ? new UserDto(sender.Id, sender.Username, sender.Email) 
                : new UserDto(Guid.Empty, "Unknown", "Unknown");

            messageDtos.Add(new MessageDto(
                message.Id,
                message.Content,
                senderDto,
                message.GroupId,
                message.CreatedAt,
                message.IsEdited,
                message.FileUrl
            ));
        }

        return messageDtos;
    }
} 