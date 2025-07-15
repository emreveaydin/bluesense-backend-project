using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Groups.Commands.KickUserFromGroup;

public class KickUserFromGroupCommandHandler : IRequestHandler<KickUserFromGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public KickUserFromGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task Handle(KickUserFromGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }

        var adminMember = group.GroupMembers.FirstOrDefault(gm => gm.UserId == request.AdminUserId);
        if (adminMember == null || adminMember.Role != GroupRole.Admin)
        {
            throw new Exception("Only admins can kick members.");
        }

        if (request.UserIdToKick == group.OwnerId)
        {
            throw new Exception("The group owner cannot be kicked.");
        }

        var memberToKick = group.GroupMembers.FirstOrDefault(gm => gm.UserId == request.UserIdToKick);
        if (memberToKick == null)
        {
            throw new Exception("User to be kicked is not a member of this group.");
        }

        if (memberToKick.Role == GroupRole.Admin && adminMember.UserId != group.OwnerId)
        {
            throw new Exception("Only the group owner can kick other admins.");
        }

        group.GroupMembers.Remove(memberToKick);

        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();
    }
} 