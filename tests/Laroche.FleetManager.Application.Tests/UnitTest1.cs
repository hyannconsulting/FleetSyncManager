namespace Laroche.FleetManager.Application.Tests;

/// <summary>
/// Tests pour valider l'infrastructure de test de la couche Application
/// </summary>
public class ApplicationTestInfrastructureTests
{
    [Fact]
    public void FluentAssertions_ShouldWork()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        value.Should().Be("test");
        value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void NSubstitute_ShouldWork()
    {
        // Arrange
        var mockRepository = Substitute.For<ITestInterface>();
        mockRepository.GetValue().Returns("mocked value");

        // Act
        var result = mockRepository.GetValue();

        // Assert
        result.Should().Be("mocked value");
        mockRepository.Received(1).GetValue();
    }

    [Fact]
    public void XUnit_BasicTest_ShouldPass()
    {
        // Arrange
        var expected = 5;
        var actual = 2 + 3;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(10, 15, 25)]
    [InlineData(-1, 1, 0)]
    public void XUnit_TheoryTest_ShouldWork(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }
}

/// <summary>
/// Interface de test pour valider NSubstitute
/// </summary>
public interface ITestInterface
{
    string GetValue();
    Task<int> GetAsync();
}
