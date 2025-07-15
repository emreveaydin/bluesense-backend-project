using Core.Application.DTOs;
using MediatR;
using System;

namespace Core.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommand : IRequest<GroupDto>
{
    public CreateGroupDto CreateGroupDto { get; set; }
    public Guid OwnerId { get; set; }

    public CreateGroupCommand(CreateGroupDto createGroupDto, Guid ownerId)
    {
        CreateGroupDto = createGroupDto;
        OwnerId = ownerId;
    }
} 