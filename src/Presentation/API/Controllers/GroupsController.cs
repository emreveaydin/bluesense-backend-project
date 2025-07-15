using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Features.Groups.Commands.CreateGroup;
using Core.Application.Features.Groups.Commands.JoinGroup;
using Core.Application.Features.Groups.Commands.LeaveGroup;
using Core.Application.Features.Groups.Commands.KickUserFromGroup;
using Core.Application.Features.Groups.Commands.InviteUserToGroup;
using Core.Application.Features.Groups.Commands.UpdateGroup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto createGroupDto)
    {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        var command = new CreateGroupCommand(createGroupDto, Guid.Parse(ownerId));
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetGroup), new { id = result.Id }, result);
    }

    [HttpPost("{groupId}/join")]
    public async Task<IActionResult> JoinGroup(Guid groupId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdValue))
        {
            return Unauthorized();
        }

        var command = new JoinGroupCommand(groupId, Guid.Parse(userIdValue));
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{groupId}/leave")]
    public async Task<IActionResult> LeaveGroup(Guid groupId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdValue))
        {
            return Unauthorized();
        }

        var command = new LeaveGroupCommand(groupId, Guid.Parse(userIdValue));
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{groupId}/kick")]
    public async Task<IActionResult> KickUser(Guid groupId, [FromBody] KickUserDto kickUserDto)
    {
        var adminIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminIdValue))
        {
            return Unauthorized();
        }

        var command = new KickUserFromGroupCommand(groupId, kickUserDto.UserId, Guid.Parse(adminIdValue));
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{groupId}/invite")]
    public async Task<ActionResult<GroupInvitationDto>> InviteUserToGroup(Guid groupId, [FromBody] InviteUserToGroupDto inviteDto)
    {
        var inviterIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(inviterIdValue))
        {
            return Unauthorized();
        }

        var command = new InviteUserToGroupCommand(groupId, inviteDto.InviteeId, Guid.Parse(inviterIdValue));
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPatch("{groupId}")]
    public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] UpdateGroupDto updateDto)
    {
        var adminIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminIdValue))
        {
            return Unauthorized();
        }

        var command = new UpdateGroupCommand(groupId, Guid.Parse(adminIdValue), updateDto);
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpGet("{id}")]
    public IActionResult GetGroup(Guid id)
    {
        // Bu, bir GetGroupQuery ile implemente edilmelidir.
        return Ok(new { Id = id, Message = "Endpoint not fully implemented." });
    }
} 