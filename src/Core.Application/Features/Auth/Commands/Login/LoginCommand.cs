using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public UserForLoginDto UserForLoginDto { get; set; }

    public LoginCommand(UserForLoginDto userForLoginDto)
    {
        UserForLoginDto = userForLoginDto;
    }
} 