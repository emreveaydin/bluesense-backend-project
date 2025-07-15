using MediatR;
using System;

namespace Core.Application.Features.Invitations.Commands.AcceptInvitation;

public record AcceptInvitationCommand(Guid InvitationId, Guid InviteeId) : IRequest; 