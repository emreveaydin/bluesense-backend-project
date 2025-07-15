using MediatR;
using System;

namespace Core.Application.Features.Messages.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid MessageId, Guid UserId) : IRequest; 