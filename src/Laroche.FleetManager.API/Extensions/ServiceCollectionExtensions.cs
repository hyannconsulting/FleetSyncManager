using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Laroche.FleetManager.API.Extensions;

/// <summary>
/// Extensions pour la configuration des services API
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre les services spécifiques à l'API
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        // Configuration de l'authentification JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("YourSuperSecretKeyThatShouldBeInConfiguration123!")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

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
