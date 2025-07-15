using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Messages.Commands.CreateMessage;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMessageBroadcastService _messageBroadcastService;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository, 
        IUserRepository userRepository, 
        IGroupRepository groupRepository, 
        IMessageBroadcastService messageBroadcastService)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _messageBroadcastService = messageBroadcastService;
    }

    public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userRepository.GetByIdAsync(request.SenderId);
        if (sender == null)
        {
            throw new Exception("Sender not found.");
        }

        var group = await _groupRepository.GetByIdAsync(request.CreateMessageDto.GroupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }
        
        var message = new Message(
            request.CreateMessageDto.Content,
            request.SenderId,
            request.CreateMessageDto.GroupId,
            request.CreateMessageDto.FileUrl
        );

        await _messageRepository.AddAsync(message);
        
        var senderDto = new UserDto(sender.Id, sender.Username, sender.Email);
        var messageDto = new MessageDto(
            message.Id,
            message.Content,
            senderDto,
            message.GroupId,
            message.CreatedAt,
            message.IsEdited,
            message.FileUrl
        );

        // Broadcast the message to all clients in the group
        await _messageBroadcastService.BroadcastMessageAsync(messageDto);

        return messageDto;
    }
} 