using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.DTOs;
using Core.Application.Features.Groups.Commands.CreateGroup;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using Core.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Features.Groups.Commands;

public class CreateGroupCommandHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandHandlerTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateGroupCommandHandler(_groupRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnGroupDto_WhenCommandIsValid()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var owner = new User("testuser", "test@test.com", new byte[0], new byte[0]);
        
        var createGroupDto = new CreateGroupDto("Test Group", "Test Description", true);
        var command = new CreateGroupCommand(createGroupDto, ownerId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(ownerId))
            .ReturnsAsync(owner);
            
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<GroupDto>();
        result.Name.Should().Be(createGroupDto.Name);

        _groupRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Group>()), Times.Once);
        _groupRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_WhenOwnerNotFound()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var createGroupDto = new CreateGroupDto("Test Group", "Test Description", true);
        var command = new CreateGroupCommand(createGroupDto, ownerId);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(ownerId))
            .ReturnsAsync(null as User);
            
        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Owner user not found.");
    }
} 