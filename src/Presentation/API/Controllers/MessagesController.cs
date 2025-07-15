using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Features.Messages.Commands.CreateMessage;
using Core.Application.Features.Messages.Queries.GetMessagesInGroup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Application.Features.Messages.Commands.EditMessage;
using Core.Application.Features.Messages.Commands.DeleteMessage;
using Core.Application.Features.Messages.Queries.SearchMessagesInGroup;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesInGroup(Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var query = new GetMessagesInGroupQuery(groupId, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("group/{groupId}/search")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> SearchMessagesInGroup(Guid groupId, [FromQuery] string searchText, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var query = new SearchMessagesInGroupQuery(groupId, searchText, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(senderId))
        {
            return Unauthorized();
        }

        var command = new CreateMessageCommand(createMessageDto, Guid.Parse(senderId));
        var result = await _mediator.Send(command);
        
        return Ok(result);
    }

    [HttpPut("{messageId}")]
    public async Task<ActionResult<MessageDto>> EditMessage(Guid messageId, [FromBody] EditMessageDto editDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new EditMessageCommand(messageId, Guid.Parse(userId), editDto.NewContent);
        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete("{messageId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var command = new DeleteMessageCommand(messageId, Guid.Parse(userId));
        await _mediator.Send(command);

        return NoContent();
    }
} 