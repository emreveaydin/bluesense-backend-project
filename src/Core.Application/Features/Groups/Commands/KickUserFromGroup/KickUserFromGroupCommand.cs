using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.KickUserFromGroup;

public record KickUserFromGroupCommand(Guid GroupId, Guid UserIdToKick, Guid AdminUserId) : IRequest; 