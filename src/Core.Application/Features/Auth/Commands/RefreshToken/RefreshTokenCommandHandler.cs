using Core.Application.DTOs;
using Core.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RefreshTokenAsync(request.RefreshToken);
    }
} 