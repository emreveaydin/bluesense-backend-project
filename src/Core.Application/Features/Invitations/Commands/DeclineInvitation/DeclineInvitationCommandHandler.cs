using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Invitations.Commands.DeclineInvitation;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand>
{
    private readonly IGroupInvitationRepository _invitationRepository;

    public DeclineInvitationCommandHandler(IGroupInvitationRepository invitationRepository)
    {
        _invitationRepository = invitationRepository;
    }

    public async Task Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId);

        if (invitation == null) throw new Exception("Invitation not found.");
        if (invitation.InviteeId != request.InviteeId) throw new Exception("This invitation is not for you.");
        if (invitation.Status != InvitationStatus.Pending) throw new Exception($"This invitation has already been {invitation.Status.ToString().ToLower()}.");
        if (invitation.ExpiresAt <= DateTime.UtcNow) throw new Exception("This invitation has expired.");

        invitation.Status = InvitationStatus.Declined;
        _invitationRepository.Update(invitation);
        await _invitationRepository.SaveChangesAsync();
    }
} 