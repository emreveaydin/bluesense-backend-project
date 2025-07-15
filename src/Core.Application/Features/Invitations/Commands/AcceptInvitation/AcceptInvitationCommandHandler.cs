using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Invitations.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand>
{
    private readonly IGroupInvitationRepository _invitationRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public AcceptInvitationCommandHandler(IGroupInvitationRepository invitationRepository, IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _invitationRepository = invitationRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId);
        if (invitation == null) throw new Exception("Invitation not found.");
        if (invitation.InviteeId != request.InviteeId) throw new Exception("This invitation is not for you.");
        if (invitation.Status != InvitationStatus.Pending) throw new Exception($"This invitation has already been {invitation.Status.ToString().ToLower()}.");
        if (invitation.ExpiresAt <= DateTime.UtcNow) throw new Exception("This invitation has expired.");

        var group = await _groupRepository.GetByIdWithMembersAsync(invitation.GroupId);
        if (group == null) throw new Exception("The group for this invitation no longer exists.");

        var user = await _userRepository.GetByIdAsync(request.InviteeId);
        if (user == null) throw new Exception("User not found.");

        if (group.GroupMembers.Any(m => m.UserId == request.InviteeId))
        {
            invitation.Status = InvitationStatus.Accepted; // Mark as accepted even if they are already a member for some reason.
            _invitationRepository.Update(invitation);
            await _invitationRepository.SaveChangesAsync();
            return; // Exit gracefully
        }

        var newMember = new GroupMember
        {
            User = user,
            Group = group,
            Role = GroupRole.Member
        };
        group.GroupMembers.Add(newMember);
        _groupRepository.Update(group);
        
        invitation.Status = InvitationStatus.Accepted;
        _invitationRepository.Update(invitation);

        await _groupRepository.SaveChangesAsync(); // This will save changes for both repositories
    }
} 