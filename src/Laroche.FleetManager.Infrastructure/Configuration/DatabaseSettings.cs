namespace Laroche.FleetManager.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for database connection and behavior
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Use in-memory database instead of PostgreSQL (for testing)
    /// </summary>
    public bool UseInMemoryDatabase { get; set; } = false;
    
    /// <summary>
    /// Automatically run migrations on startup
    /// </summary>
    public bool AutoMigrate { get; set; } = true;
    
    /// <summary>
    /// Seed sample data during initialization
    /// </summary>
    public bool SeedSampleData { get; set; } = false;
    
    /// <summary>
    /// Enable sensitive data logging for development
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
    
    /// <summary>
    /// Connection timeout in seconds
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30;
    
    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    public int CommandTimeout { get; set; } = 60;
    
    /// <summary>
    /// Maximum retry count for database operations
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;
    
    /// <summary>
    /// Maximum retry delay between attempts
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Enable detailed errors in development
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
    
    /// <summary>
    /// Enable service provider validation in development
    /// </summary>
    public bool EnableServiceProviderValidation { get; set; } = false;
}
