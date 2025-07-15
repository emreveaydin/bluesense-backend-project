using Moq;
using FluentAssertions;
using Core.Application.Features.Auth.Commands.Register;
using Core.Application.Interfaces;
using Core.Application.DTOs;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Application.UnitTests.Features.Auth.Commands.Register;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new RegisterCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponseDto_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registerDto = new UserForRegisterDto("newuser", "new@example.com", "password123");
        var command = new RegisterCommand(registerDto);
        var expectedResponse = new AuthResponseDto("accesstoken", "refreshtoken");

        _authServiceMock
            .Setup(s => s.RegisterAsync(registerDto))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResponse);
        _authServiceMock.Verify(s => s.RegisterAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRegistrationFails()
    {
        // Arrange
        var registerDto = new UserForRegisterDto("existinguser", "existing@example.com", "password123");
        var command = new RegisterCommand(registerDto);
        var expectedException = new Exception("Registration failed.");
        
        _authServiceMock
            .Setup(s => s.RegisterAsync(registerDto))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Registration failed.");
        _authServiceMock.Verify(s => s.RegisterAsync(registerDto), Times.Once);
    }
} 