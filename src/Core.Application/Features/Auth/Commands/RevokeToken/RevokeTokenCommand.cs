using MediatR;

namespace Core.Application.Features.Auth.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<bool>; 