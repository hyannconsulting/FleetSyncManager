using Laroche.FleetManager.Domain.Constants;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service d'initialisation des données d'authentification
/// Crée les rôles par défaut et l'utilisateur administrateur initial
/// </summary>
public static class AuthenticationSeeder
{
    /// <summary>
    /// Initialise les rôles et l'utilisateur administrateur par défaut
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("AuthenticationSeeder");

        try
        {
            // Créer les rôles
            await CreateRoleIfNotExistsAsync(roleManager, UserRoles.Admin, logger);
            await CreateRoleIfNotExistsAsync(roleManager, UserRoles.FleetManager, logger);
            await CreateRoleIfNotExistsAsync(roleManager, UserRoles.Driver, logger);

            // Créer l'utilisateur administrateur par défaut
            await CreateDefaultAdminAsync(userManager, logger);
            await CreateDefaultDriverAsync(userManager, logger);
            await CreateDefaultFleetManagerAsync(userManager, logger);

            logger.LogInformation("Initialisation des données d'authentification terminée avec succès");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'initialisation des données d'authentification");
            throw;
        }
    }

    private static async Task CreateRoleIfNotExistsAsync(RoleManager<IdentityRole> roleManager, string roleName, ILogger logger)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                logger.LogInformation("Rôle {RoleName} créé avec succès", roleName);
            }
            else
            {
                logger.LogError("Erreur lors de la création du rôle {RoleName}: {Errors}",
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogDebug("Rôle {RoleName} existe déjà", roleName);
        }
    }

    private static async Task CreateDefaultDriverAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        const string adminEmail = "driversystem@fleetsyncmanager.com";
        const string adminPassword = "Admin123!@#";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "driver",
                LastName = "Système",
                Status = UserStatus.Active,
                MustChangePassword = false
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Ajouter le rôle Admin
                await userManager.AddToRoleAsync(adminUser, UserRoles.Driver);

                logger.LogInformation("Utilisateur driver par défaut créé: {Email}", adminEmail);
                logger.LogWarning("SÉCURITÉ: Changez le mot de passe du driver par défaut dès la première connexion!");
            }
            else
            {
                logger.LogError("Erreur lors de la création de l'utilisateur driver : {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogDebug("Utilisateur administrateur existe déjà");
        }
    }

    private static async Task CreateDefaultAdminAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        const string adminEmail = "admin@fleetsyncmanager.com";
        const string adminPassword = "Admin123!@#";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Administrateur",
                LastName = "Système",
                Status = UserStatus.Active,
                MustChangePassword = true // Forcer le changement au premier login
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Ajouter le rôle Admin
                await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

                logger.LogInformation("Utilisateur administrateur par défaut créé: {Email}", adminEmail);
                logger.LogWarning("SÉCURITÉ: Changez le mot de passe de l'administrateur par défaut dès la première connexion!");
            }
            else
            {
                logger.LogError("Erreur lors de la création de l'utilisateur administrateur: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogDebug("Utilisateur administrateur existe déjà");
        }
    }

    private static async Task CreateDefaultFleetManagerAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        const string adminEmail = "admin@fleetsyncmanager.com";
        const string adminPassword = "Admin123!@#";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "FleetManager",
                LastName = "Système",
                Status = UserStatus.Active,
                MustChangePassword = true // Forcer le changement au premier login
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Ajouter le rôle Admin
                await userManager.AddToRoleAsync(adminUser, UserRoles.FleetManager);

                logger.LogInformation("Utilisateur FleetManager par défaut créé: {Email}", adminEmail);
                logger.LogWarning("SÉCURITÉ: Changez le mot de passe de l'administrateur par défaut dès la première connexion!");
            }
            else
            {
                logger.LogError("Erreur lors de la création de l'FleetManager administrateur: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            logger.LogDebug("Utilisateur FleetManager existe déjà");
        }
    }
}
