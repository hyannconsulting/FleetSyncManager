using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace Laroche.FleetManager.API.Tests;

/// <summary>
/// Tests d'infrastructure pour valider que tous les frameworks de test API fonctionnent correctement
/// </summary>
public class ApiTestInfrastructureTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiTestInfrastructureTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public void WebApplicationFactory_ShouldBeInitialized()
    {
        // Arrange & Act & Assert
        _factory.Should().NotBeNull();
    }

    [Fact]
    public void HttpClient_ShouldBeCreated()
    {
        // Arrange & Act & Assert
        _client.Should().NotBeNull();
        _client.BaseAddress.Should().NotBeNull();
    }

    [Fact]
    public void FluentAssertions_ShouldWork()
    {
        // Arrange
        var testValue = "API Test";

        // Act & Assert
        testValue.Should().NotBeEmpty();
        testValue.Should().Contain("Test");
        testValue.Length.Should().BeGreaterThan(5);
    }

    [Fact]
    public void NSubstitute_ShouldWork()
    {
        // Arrange
        var mockLogger = Substitute.For<ILogger<ApiTestInfrastructureTests>>();

        // Act
        mockLogger.LogInformation("Test message");

        // Assert
        mockLogger.Received(1).LogInformation("Test message");
    }

    [Fact]
    public void JsonSerialization_ShouldWork()
    {
        // Arrange
        var testObject = new { Id = 1, Name = "Test API", IsActive = true };

        // Act
        var json = JsonSerializer.Serialize(testObject);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        json.Should().NotBeEmpty();
        json.Should().Contain("Test API");
        jsonElement.ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task HttpClient_ShouldMakeRequests()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/");

        // Assert
        response.Should().NotBeNull();
        // Note: On s'attend à une redirection ou une page d'erreur car aucune route "/" n'est définie
    }

    [Fact]
    public void ServiceProvider_ShouldResolveServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetService<ILogger<ApiTestInfrastructureTests>>();

        // Assert
        serviceProvider.Should().NotBeNull();
        logger.Should().NotBeNull();
    }

    [Fact]
    public void WebHost_ShouldBeConfigured()
    {
        // Arrange & Act
        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();

        // Assert
        environment.Should().NotBeNull();
        environment.EnvironmentName.Should().NotBeEmpty();
    }

    [Fact]
    public async Task API_ShouldHandleInvalidRoutes()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/nonexistent");

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
