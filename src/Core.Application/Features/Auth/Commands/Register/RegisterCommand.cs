using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public UserForRegisterDto UserForRegisterDto { get; set; }

    public RegisterCommand(UserForRegisterDto userForRegisterDto)
    {
        UserForRegisterDto = userForRegisterDto;
    }
} 