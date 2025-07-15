using Core.Application.Interfaces;
using MediatR;
using Core.Application.DTOs;

namespace Core.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(request.UserForRegisterDto);
    }
} 