using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Groups.Commands.UpdateGroup;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand>
{
    private readonly IGroupRepository _groupRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId);
        if (group == null)
        {
            throw new Exception("Group not found.");
        }

        var adminMember = group.GroupMembers.FirstOrDefault(gm => gm.UserId == request.AdminId);
        if (adminMember == null || adminMember.Role != GroupRole.Admin)
        {
            throw new Exception("Only admins can update group information.");
        }
        
        // This is a private method on the entity, which is not ideal for this architecture.
        // A better approach would be public methods on the entity like UpdateName(string name).
        // For now, to update the properties, I will need to make the setters public or add a method.
        // Let's assume there is a method on the Group entity to update its properties.
        // I will add a method to the Group entity to handle this.
        
        group.UpdateDetails(request.Dto.Name, request.Dto.Description, request.Dto.IsPublic);
        
        _groupRepository.Update(group);
        await _groupRepository.SaveChangesAsync();
    }
} 