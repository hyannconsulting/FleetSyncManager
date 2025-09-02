using Laroche.FleetManager.API.Endpoints;

namespace Laroche.FleetManager.API.Extensions;

/// <summary>
/// Extensions pour la configuration des endpoints de l'API
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configure tous les endpoints de l'API
    /// </summary>
    public static WebApplication ConfigureApiEndpoints(this WebApplication app)
    {
        // Configuration middleware d'authentification
        app.UseAuthentication();
        app.UseAuthorization();

        // Endpoints d'authentification
        app.MapAuthEndpoints();

        // Configuration des endpoints par domaine métier
        app.ConfigureVehicleEndpoints();
        app.ConfigureDriverEndpoints();
        app.ConfigureMaintenanceEndpoints();
        app.ConfigureIncidentEndpoints();

        return app;
    }
}
