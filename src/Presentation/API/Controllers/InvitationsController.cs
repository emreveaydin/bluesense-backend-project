using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Application.Features.Invitations.Commands.AcceptInvitation;
using Core.Application.Features.Invitations.Commands.DeclineInvitation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/invitations")]
public class InvitationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{invitationId}/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid invitationId)
    {
        var inviteeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(inviteeIdValue))
        {
            return Unauthorized();
        }

        var command = new AcceptInvitationCommand(invitationId, Guid.Parse(inviteeIdValue));
        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost("{invitationId}/decline")]
    public async Task<IActionResult> DeclineInvitation(Guid invitationId)
    {
        var inviteeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(inviteeIdValue))
        {
            return Unauthorized();
        }

        var command = new DeclineInvitationCommand(invitationId, Guid.Parse(inviteeIdValue));
        await _mediator.Send(command);

        return NoContent();
    }
} 