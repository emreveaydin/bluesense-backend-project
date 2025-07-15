using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Interfaces;
using Core.Domain.Contracts;
using MediatR;

namespace Core.Application.Features.Messages.Commands.DeleteMessage;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageBroadcastService _broadcastService;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository, IMessageBroadcastService broadcastService)
    {
        _messageRepository = messageRepository;
        _broadcastService = broadcastService;
    }

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.MessageId);
        if (message == null) throw new Exception("Message not found.");
        
        // Add logic here to allow group admins to delete messages as well
        if (message.SenderId != request.UserId)
        {
            throw new Exception("You can only delete your own messages.");
        }

        if (message.IsDeleted) return; // Already deleted, no action needed.

        message.SoftDelete();
        _messageRepository.Update(message);
        await _messageRepository.SaveChangesAsync();

        await _broadcastService.MessageDeleted(message.GroupId, message.Id);
    }
} 