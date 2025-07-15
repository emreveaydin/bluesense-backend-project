using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> LoginAsync(UserForLoginDto userForLoginDto)
    {
        var user = await _userRepository.GetByEmailAsync(userForLoginDto.Email);

        if (user == null || !VerifyPasswordHash(userForLoginDto.Password, user.PasswordHash, user.PasswordSalt))
        {
            // In a real app, use a custom exception type
            throw new Exception("Invalid credentials.");
        }

        return await GenerateTokensAndUpdateUser(user);
    }

    public async Task<AuthResponseDto> RegisterAsync(UserForRegisterDto userForRegisterDto)
    {
        if (await _userRepository.GetByEmailAsync(userForRegisterDto.Email) != null)
        {
            throw new Exception("Email is already taken.");
        }

        CreatePasswordHash(userForRegisterDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User(userForRegisterDto.Username, userForRegisterDto.Email, passwordHash, passwordSalt);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return await GenerateTokensAndUpdateUser(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = (await _userRepository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new Exception("Invalid or expired refresh token.");
        }

        return await GenerateTokensAndUpdateUser(user);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var user = (await _userRepository.FindAsync(u => u.RefreshToken == refreshToken)).FirstOrDefault();
        if (user == null) return false;

        user.SetRefreshToken(null, DateTime.UtcNow);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }
    
    private async Task<AuthResponseDto> GenerateTokensAndUpdateUser(User user)
    {
        var accessToken = GenerateJwtToken(user);
        var (refreshTokenValue, refreshTokenExpires) = GenerateRefreshToken();

        user.SetRefreshToken(refreshTokenValue, refreshTokenExpires);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshTokenValue);
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private (string Token, DateTime Expires) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return (
            Convert.ToBase64String(randomNumber),
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays)
        );
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
} 