using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact(Skip = ".NET 9 preview version incompatibility, will be re-enabled. | .NET 9 preview sürüm uyumsuzluğu nedeniyle geçici olarak atlandı, ileride etkinleştirilecek.")]
    public async Task Register_ShouldReturnSuccessAndTokens_WhenDataIsValid()
    {
        // Arrange
        var registerDto = new UserForRegisterDto(
            $"testuser{Guid.NewGuid()}@test.com", 
            "testuser", 
            "Password123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var authResponse = await System.Text.Json.JsonSerializer.DeserializeAsync<AuthResponseDto>(responseStream,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        authResponse.Should().NotBeNull();
        authResponse!.AccessToken.Should().NotBeNullOrEmpty();
        authResponse.RefreshToken.Should().NotBeNullOrEmpty();
    }
} 