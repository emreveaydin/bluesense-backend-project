using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Contracts;
using MediatR;

namespace Core.Application.Features.Messages.Commands.EditMessage;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMessageBroadcastService _broadcastService;

    public EditMessageCommandHandler(IMessageRepository messageRepository, IUserRepository userRepository, IMessageBroadcastService broadcastService)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _broadcastService = broadcastService;
    }

    public async Task<MessageDto> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId);
        if (message == null) throw new Exception("Message not found.");
        if (message.SenderId != request.UserId) throw new Exception("You can only edit your own messages.");
        if (message.IsDeleted) throw new Exception("Cannot edit a deleted message.");

        message.UpdateContent(request.NewContent);
        
        _messageRepository.Update(message);
        // For Mongo, SaveChangesAsync is a no-op, but we call it for consistency with IGenericRepository if it were used elsewhere.
        await _messageRepository.SaveChangesAsync(); 

        var sender = await _userRepository.GetByIdAsync(message.SenderId);
        if (sender == null) throw new Exception("Message sender not found.");
        var senderDto = new UserDto(sender.Id, sender.Username, sender.Email);

        var messageDto = new MessageDto(message.Id, message.Content, senderDto, message.GroupId, message.CreatedAt, message.IsEdited, message.FileUrl);

        await _broadcastService.MessageEdited(message.GroupId, messageDto);

        return messageDto;
    }
} 