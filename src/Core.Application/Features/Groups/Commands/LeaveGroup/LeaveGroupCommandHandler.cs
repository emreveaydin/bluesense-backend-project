using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using MediatR;

namespace Core.Application.Features.Groups.Commands.LeaveGroup;

public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public LeaveGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }

        if (group.OwnerId == request.UserId)
        {
            throw new Exception("The owner cannot leave the group. You must delete the group or transfer ownership.");
        }

        var memberToRemove = group.GroupMembers.FirstOrDefault(gm => gm.UserId == request.UserId);
        if (memberToRemove == null)
        {
            throw new Exception("User is not a member of this group.");
        }

        group.GroupMembers.Remove(memberToRemove);
        
        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();
    }
} 