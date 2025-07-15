using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserForRegisterDto userForRegisterDto);
    Task<AuthResponseDto> LoginAsync(UserForLoginDto userForLoginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> RevokeTokenAsync(string refreshToken);
} 