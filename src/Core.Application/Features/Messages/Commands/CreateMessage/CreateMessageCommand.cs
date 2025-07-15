using Core.Application.DTOs;
using MediatR;
using System;

namespace Core.Application.Features.Messages.Commands.CreateMessage;

public class CreateMessageCommand : IRequest<MessageDto>
{
    public CreateMessageDto CreateMessageDto { get; set; }
    public Guid SenderId { get; set; }

    public CreateMessageCommand(CreateMessageDto createMessageDto, Guid senderId)
    {
        CreateMessageDto = createMessageDto;
        SenderId = senderId;
    }
} 