namespace Core.Application.DTOs;

public record UserForRegisterDto(string Username, string Email, string Password);

public record UserForLoginDto(string Email, string Password);

public record AuthResponseDto(string AccessToken, string RefreshToken);

public record RefreshTokenRequestDto(string RefreshToken); 