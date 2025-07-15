using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.LeaveGroup;

public record LeaveGroupCommand(Guid GroupId, Guid UserId) : IRequest; 