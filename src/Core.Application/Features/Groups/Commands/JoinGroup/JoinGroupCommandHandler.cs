using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Groups.Commands.JoinGroup;

public class JoinGroupCommandHandler : IRequestHandler<JoinGroupCommand>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public JoinGroupCommandHandler(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(JoinGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }

        if (!group.IsPublic)
        {
            throw new Exception("This group is private and cannot be joined directly.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (group.GroupMembers.Any(gm => gm.UserId == request.UserId))
        {
            throw new Exception("User is already a member of this group.");
        }

        var newMember = new GroupMember
        {
            Group = group,
            User = user,
            Role = GroupRole.Member
        };

        group.GroupMembers.Add(newMember);
        
        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();
    }
} 