using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Groups.Commands.InviteUserToGroup;

public class InviteUserToGroupCommandHandler : IRequestHandler<InviteUserToGroupCommand, GroupInvitationDto>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGroupInvitationRepository _invitationRepository;

    public InviteUserToGroupCommandHandler(IGroupRepository groupRepository, IUserRepository userRepository, IGroupInvitationRepository invitationRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _invitationRepository = invitationRepository;
    }

    public async Task<GroupInvitationDto> Handle(InviteUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId);
        if (group == null) throw new Exception("Group not found.");

        if (group.IsPublic) throw new Exception("Cannot invite users to a public group. They can join directly.");

        var inviter = group.GroupMembers.FirstOrDefault(m => m.UserId == request.InviterId);
        if (inviter == null || inviter.Role != GroupRole.Admin)
        {
            throw new Exception("Only admins can invite users to this group.");
        }

        var invitee = await _userRepository.GetByIdAsync(request.InviteeId);
        if (invitee == null) throw new Exception("User to be invited not found.");

        if (group.GroupMembers.Any(m => m.UserId == request.InviteeId))
        {
            throw new Exception("User is already a member of this group.");
        }

        var existingInvitation = await _invitationRepository.FindAsync(i =>
            i.GroupId == request.GroupId &&
            i.InviteeId == request.InviteeId &&
            i.Status == InvitationStatus.Pending);
            
        if (existingInvitation.Any())
        {
            throw new Exception("An invitation has already been sent to this user for this group.");
        }

        var invitation = new GroupInvitation(request.GroupId, request.InviterId, request.InviteeId, TimeSpan.FromDays(7));

        await _invitationRepository.AddAsync(invitation);
        await _invitationRepository.SaveChangesAsync();

        return new GroupInvitationDto(invitation.Id, invitation.GroupId, invitation.InviteeId, invitation.InviterId, invitation.Status, invitation.ExpiresAt);
    }
} 