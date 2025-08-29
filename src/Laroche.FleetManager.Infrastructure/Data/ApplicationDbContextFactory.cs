using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Laroche.FleetManager.Infrastructure.Data;

/// <summary>
/// Factory pour créer le DbContext au moment du design (migrations, scaffolding, etc.)
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Crée une instance du contexte de base de données pour les opérations de design-time
    /// </summary>
    /// <param name="args">Arguments de ligne de commande</param>
    /// <returns>Instance configurée du contexte</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Configuration pour les migrations - utilise PostgreSQL
        var connectionString = "Host=localhost;Port=5432;Database=fleetmanager_dev;Username=fleetmanager;Password=DevPassword123!;Include Error Detail=true;Pooling=true;Minimum Pool Size=1;Maximum Pool Size=20;";
        
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsAssembly("Laroche.FleetManager.Infrastructure");
        });
        
        // Configuration pour le développement
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
        
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
