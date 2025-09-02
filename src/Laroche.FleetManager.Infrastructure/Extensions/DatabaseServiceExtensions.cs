using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Infrastructure.Configuration;
using Laroche.FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laroche.FleetManager.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions pour la configuration des services de base de données
    /// </summary>
    public static class DatabaseServiceExtensions
    {
        /// <summary>
        /// Ajouter les services de base de données à la DI
        /// </summary>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration des paramètres de base de données
            var dbSettings = new DatabaseSettings();
            configuration.GetSection("DatabaseSettings").Bind(dbSettings);
            services.Configure<DatabaseSettings>(options => configuration.GetSection("DatabaseSettings").Bind(options));

            // Configuration du contexte de base de données
            if (dbSettings.UseInMemoryDatabase)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(configuration.GetConnectionString("InMemoryConnection") ?? "InMemoryFleetManager");

                    if (dbSettings.EnableSensitiveDataLogging)
                    {
                        options.EnableSensitiveDataLogging();
                    }

                    if (dbSettings.EnableDetailedErrors)
                    {
                        options.EnableDetailedErrors();
                    }
                });
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.CommandTimeout(dbSettings.CommandTimeout);
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: dbSettings.MaxRetryCount,
                            maxRetryDelay: dbSettings.MaxRetryDelay,
                            errorCodesToAdd: null);
                    });

                    if (dbSettings.EnableSensitiveDataLogging)
                    {
                        options.EnableSensitiveDataLogging();
                    }

                    if (dbSettings.EnableDetailedErrors)
                    {
                        options.EnableDetailedErrors();
                    }
                });
            }

            return services;
        }

        /// <summary>
        /// Initialiser la base de données (migrations et données d'exemple)
        /// </summary>
        public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                var configuration = services.GetRequiredService<IConfiguration>();

                var dbSettings = new DatabaseSettings();
                configuration.GetSection("DatabaseSettings").Bind(dbSettings);

                // Appliquer les migrations si activé
                if (dbSettings.AutoMigrate && !dbSettings.UseInMemoryDatabase)
                {
                    logger.LogInformation("Application des migrations de base de données...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations appliquées avec succès.");
                }

                // Seed des données d'exemple si activé
                if (dbSettings.SeedSampleData)
                {
                    await SeedSampleDataAsync(context, logger);
                }

                logger.LogInformation("Initialisation de la base de données terminée.");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "Erreur lors de l'initialisation de la base de données");
                throw;
            }

            return serviceProvider;
        }

        /// <summary>
        /// Seeder les données d'exemple pour le développement
        /// </summary>
        private static async Task SeedSampleDataAsync(ApplicationDbContext context, ILogger logger)
        {
            if (context.Vehicles.Any() || context.Drivers.Any())
            {
                logger.LogInformation("Des données existent déjà, pas de seed nécessaire.");
                return;
            }

            logger.LogInformation("Création des données d'exemple...");

            try
            {
                // Création de conducteurs d'exemple
                var drivers = new List<Driver>
                {
                    new() {
                        FirstName = "Jean",
                        LastName = "Dupont",
                        Email = "jean.dupont@fleetmanager.com",
                        LicenseNumber = "DUPONT123456",
                        Address = "123 Rue de la Paix",
                        CreatedAt = DateTime.UtcNow
                    },
                    new()
                    {
                        FirstName = "Marie",
                        LastName = "Martin",
                        Email = "marie.martin@fleetmanager.com",
                        LicenseNumber = "MARTIN123456",
                        Address = "456 Avenue des Champs",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Drivers.AddRange(drivers);
                await context.SaveChangesAsync();

                // Création de véhicules d'exemple
                var vehicles = new List<Vehicle>
                {
                    new Vehicle
                    {
                        LicensePlate = "AB-123-CD",
                        Brand = "Peugeot",
                        Model = "308",
                        Year = 2020,
                        CurrentMileage = 45000,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Vehicle
                    {
                        LicensePlate = "EF-456-GH",
                        Brand = "Citroën",
                        Model = "C4",
                        Year = 2019,
                        CurrentMileage = 67000,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Vehicles.AddRange(vehicles);
                await context.SaveChangesAsync();

                logger.LogInformation("Données d'exemple créées avec succès: {DriverCount} conducteurs, {VehicleCount} véhicules",
                    drivers.Count, vehicles.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la création des données d'exemple");
                throw;
            }
        }
    }
}
