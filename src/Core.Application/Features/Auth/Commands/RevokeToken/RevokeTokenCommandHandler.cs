using Core.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Features.Auth.Commands.RevokeToken;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
{
    private readonly IAuthService _authService;

    public RevokeTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RevokeTokenAsync(request.RefreshToken);
    }
} 