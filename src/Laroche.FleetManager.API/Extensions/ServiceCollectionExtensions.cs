using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Infrastructure.Data;
using Laroche.FleetManager.Infrastructure.Extensions;
using Laroche.FleetManager.Infrastructure.Repositories;
using Laroche.FleetManager.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Laroche.FleetManager.API.Extensions;

/// <summary>
/// Extensions pour la configuration des services de l'API
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configure les services d'authentification pour l'API
    /// </summary>
    public static async Task<IServiceCollection> AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuration de la base de données
        services.AddDatabaseServices(configuration);

        // Configuration IHttpContextAccessor
        services.AddHttpContextAccessor();

        // Configuration ASP.NET Core Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Configuration mots de passe
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // Configuration verrouillage
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Configuration utilisateur
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

            // Configuration connexion
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configuration JWT Bearer
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "your-super-secret-jwt-key-here-must-be-at-least-256-bits");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true; // En production, mettre à true
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"] ?? "FleetSyncManager",
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"] ?? "FleetSyncManagerAPI",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configuration autorisation
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("FleetManagerPolicy", policy =>
                policy.RequireRole("Administrator", "FleetManager"));

            options.AddPolicy("DriverPolicy", policy =>
                policy.RequireRole("Administrator", "FleetManager", "Driver"));
        });

        // Enregistrement des services d'authentification
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ILoginAuditService, LoginAuditService>();
        services.AddScoped<IUserService, UserService>();

        // Repositories
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();

        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    /// <summary>
    /// Enregistre les services spécifiques à l'API
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Configuration MediatR pour scanner l'assembly Application
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Laroche.FleetManager.Application.Queries.Vehicles.GetVehiclesQuery>();
        });

        // Configuration AutoMapper
        services.AddAutoMapper(
            typeof(Laroche.FleetManager.Application.Mappings.VehicleMappingProfile),
            typeof(Laroche.FleetManager.Application.Mappings.DriverMappingProfile)
        );

        // Configuration des contrôleurs API
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                // Configuration personnalisée pour les erreurs de validation
                options.SuppressModelStateInvalidFilter = false;
            });

        return services;
    }
}
