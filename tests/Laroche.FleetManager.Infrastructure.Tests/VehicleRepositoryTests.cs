using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Laroche.FleetManager.Infrastructure.Tests;

/// <summary>
/// Tests pour valider l'infrastructure de test de la couche Infrastructure
/// </summary>
public class InfrastructureTestInfrastructureTests
{
    [Fact]
    public void EfCoreInMemoryDatabase_ShouldWork()
    {
        // Arrange
        var options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act & Assert
        using var context = new DbContext(options);
        context.Should().NotBeNull();
        context.Database.Should().NotBeNull();
    }

    [Fact]
    public void EfCoreSqliteDatabase_ShouldWork()
    {
        // Arrange
        var options = new DbContextOptionsBuilder()
            .UseSqlite("DataSource=:memory:")
            .Options;

        // Act & Assert
        using var context = new DbContext(options);
        context.Should().NotBeNull();
        context.Database.Should().NotBeNull();
    }

    [Fact]
    public async Task InMemoryDatabase_CanCreateAndQuery()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act & Assert
        await using var context = new TestDbContext(options);
        await context.Database.EnsureCreatedAsync();

        // Add test data
        var testEntity = new TestEntity { Name = "Test", Value = 42 };
        context.TestEntities.Add(testEntity);
        await context.SaveChangesAsync();

        // Query data
        var retrieved = await context.TestEntities.FirstOrDefaultAsync(x => x.Name == "Test");
        retrieved.Should().NotBeNull();
        retrieved!.Value.Should().Be(42);
    }

    [Fact]
    public void FluentAssertions_ShouldWork()
    {
        // Arrange
        var value = "infrastructure test";

        // Act & Assert
        value.Should().Be("infrastructure test");
        value.Should().Contain("infrastructure");
    }

    [Fact]
    public async Task NSubstitute_MockingRepository_ShouldWork()
    {
        // Arrange
        var mockRepo = Substitute.For<ITestRepository>();
        mockRepo.GetByIdAsync(1).Returns(Task.FromResult(new TestEntity { Id = 1, Name = "Test" }));

        // Act
        var result = await mockRepo.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
    }
}

/// <summary>
/// Context de test simple pour EF Core
/// </summary>
public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<TestEntity> TestEntities { get; set; } = null!;
}

/// <summary>
/// Entité de test simple
/// </summary>
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

/// <summary>
/// Interface repository de test
/// </summary>
public interface ITestRepository
{
    Task<TestEntity> GetByIdAsync(int id);
}
