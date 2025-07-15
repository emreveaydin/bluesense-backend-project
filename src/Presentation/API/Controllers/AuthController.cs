using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Features.Auth.Commands.Login;
using Core.Application.Features.Auth.Commands.Register;
using Core.Application.Features.Auth.Commands.RefreshToken;
using Core.Application.Features.Auth.Commands.RevokeToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(UserForRegisterDto registerDto)
    {
        var command = new RegisterCommand(registerDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(UserForLoginDto loginDto)
    {
        var command = new LoginCommand(loginDto);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken(RefreshTokenRequestDto request)
    {
        var command = new RevokeTokenCommand(request.RefreshToken);
        var success = await _mediator.Send(command);
        if (success)
        {
            return NoContent();
        }
        return BadRequest("Invalid token.");
    }
} 