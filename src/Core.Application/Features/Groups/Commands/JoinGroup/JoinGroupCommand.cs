using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.JoinGroup;

public record JoinGroupCommand(Guid GroupId, Guid UserId) : IRequest; 