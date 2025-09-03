namespace Laroche.FleetManager.API.Endpoints;

/// <summary>
/// Configuration des endpoints pour la gestion des conducteurs
/// </summary>
public static class DriverEndpoints
{
    /// <summary>
    /// Configure les endpoints pour les conducteurs
    /// </summary>
    public static WebApplication ConfigureDriverEndpoints(this WebApplication app)
    {
        var driverGroup = app.MapGroup("/api/v1/drivers")
            .WithTags("Drivers")
            .WithOpenApi();

        // GET /api/v1/drivers - Version simplifiée pour test
        driverGroup.MapGet("/", () =>
        {
            return Results.Ok(new { message = "API Conducteurs opérationnelle", timestamp = DateTime.UtcNow });
        })
        .WithName("GetDrivers")
        .WithSummary("Récupère la liste des conducteurs")
        .WithDescription("Endpoint de test - retourne un message de confirmation")
        .Produces<object>(StatusCodes.Status200OK)
        // .RequireAuthorization()
        ;

        return app;
    }
}
