using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using MediatR;
using Core.Domain.Enums;

namespace Core.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupDto>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GroupDto> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId);
        if (owner == null)
        {
            // In a real app, use a custom, more specific exception
            throw new Exception("Owner user not found.");
        }

        var group = new Group(
            request.CreateGroupDto.Name,
            request.OwnerId,
            request.CreateGroupDto.Description,
            request.CreateGroupDto.IsPublic
        );

        var adminMember = new GroupMember
        {
            User = owner,
            Group = group,
            Role = GroupRole.Admin
        };

        group.GroupMembers.Add(adminMember);

        await _groupRepository.AddAsync(group);
        await _groupRepository.SaveChangesAsync();

        return new GroupDto(group.Id, group.Name, group.Description, group.IsPublic, group.OwnerId);
    }
} 