using Core.Application.DTOs;
using MediatR;
using System;

namespace Core.Application.Features.Messages.Commands.EditMessage;

public record EditMessageCommand(Guid MessageId, Guid UserId, string NewContent) : IRequest<MessageDto>; 