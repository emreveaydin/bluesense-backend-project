using MediatR;
using System;

namespace Core.Application.Features.Invitations.Commands.DeclineInvitation;

public record DeclineInvitationCommand(Guid InvitationId, Guid InviteeId) : IRequest; 