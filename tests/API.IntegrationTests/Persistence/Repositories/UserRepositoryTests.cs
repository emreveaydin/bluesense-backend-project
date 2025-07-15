using System.Threading.Tasks;
using Core.Domain.Contracts;
using Core.Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence.Postgres.Context;
using Infrastructure.Persistence.Postgres.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.IntegrationTests.Persistence.Repositories;

public class UserRepositoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public UserRepositoryTests(CustomWebApplicationFactory factory)
    {
        _scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userRepository = new UserRepository(context);

        var passwordHash = new byte[] { 1, 2, 3 };
        var passwordSalt = new byte[] { 4, 5, 6 };
        var newUser = new User("testuser", "test@test.com", passwordHash, passwordSalt);
        await userRepository.AddAsync(newUser);
        await context.SaveChangesAsync();

        // Act
        var foundUser = await userRepository.GetByUsernameAsync("testuser");

        // Assert
        foundUser.Should().NotBeNull();
        foundUser.Email.Should().Be("test@test.com");
    }
} 