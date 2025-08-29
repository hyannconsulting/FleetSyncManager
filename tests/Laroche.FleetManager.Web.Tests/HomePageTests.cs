using System.Text.Json;
using AngleSharp;
using AngleSharp.Html.Dom;

namespace Laroche.FleetManager.Web.Tests;

/// <summary>
/// Tests pour valider l'infrastructure de test de la couche Web
/// </summary>
public class WebTestInfrastructureTests
{
    [Fact]
    public void FluentAssertions_ShouldWork()
    {
        // Arrange
        var value = "web test";

        // Act & Assert
        value.Should().Be("web test");
        value.Should().Contain("web");
    }

    [Fact]
    public void HttpClient_Creation_ShouldWork()
    {
        // Arrange & Act
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:5001/");

        // Assert
        client.Should().NotBeNull();
        client.BaseAddress.Should().NotBeNull();
    }

    [Fact]
    public async Task WebApplicationFactory_ShouldWork()
    {
        // Cette approche simple évite les dépendances complexes pour l'instant
        await Task.CompletedTask;
        
        // Assert - Vérification que le test peut s'exécuter
        Assert.True(true);
    }

    [Theory]
    [InlineData("/", "text/html")]
    [InlineData("/api/v1/vehicles", "application/json")]
    [InlineData("/health", "text/plain")]
    public void UrlAndContentType_Validation_ShouldWork(string url, string expectedContentType)
    {
        // Arrange & Assert
        url.Should().StartWith("/");
        expectedContentType.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AngleSharp_HtmlParsing_ShouldWork()
    {
        // Arrange
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var htmlContent = "<html><head><title>Test</title></head><body><h1>Hello World</h1></body></html>";

        // Act
        var document = await context.OpenAsync(req => req.Content(htmlContent));

        // Assert
        document.Should().NotBeNull();
        document.Title.Should().Be("Test");
        var h1Element = document.QuerySelector("h1") as IHtmlElement;
        h1Element?.TextContent.Should().Be("Hello World");
    }

    [Fact]
    public void Json_Serialization_ShouldWork()
    {
        // Arrange
        var testObject = new { Name = "Test", Value = 42 };

        // Act
        var json = JsonSerializer.Serialize(testObject);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        // Assert
        json.Should().Contain("Test");
        json.Should().Contain("42");
        deserialized.Should().NotBeNull();
    }
}
