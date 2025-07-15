using Moq;
using FluentAssertions;
using Core.Application.Features.Auth.Commands.Login;
using Core.Application.Interfaces;
using Core.Application.DTOs;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Application.UnitTests.Features.Auth.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new LoginCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponseDto_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDto = new UserForLoginDto("test@example.com", "password123");
        var command = new LoginCommand(loginDto);
        var expectedResponse = new AuthResponseDto("accesstoken", "refreshtoken");

        _authServiceMock
            .Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        _authServiceMock.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenLoginFails()
    {
        // Arrange
        var loginDto = new UserForLoginDto("wrong@example.com", "wrongpassword");
        var command = new LoginCommand(loginDto);
        
        _authServiceMock
            .Setup(s => s.LoginAsync(loginDto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid credentials");
        _authServiceMock.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }
} 