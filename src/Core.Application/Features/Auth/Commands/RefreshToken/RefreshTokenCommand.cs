using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>; 