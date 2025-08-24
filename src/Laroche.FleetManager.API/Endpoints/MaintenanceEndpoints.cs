namespace Laroche.FleetManager.API.Endpoints;

/// <summary>
/// Configuration des endpoints pour la gestion de la maintenance
/// </summary>
public static class MaintenanceEndpoints
{
    /// <summary>
    /// Configure les endpoints pour la maintenance
    /// </summary>
    public static WebApplication ConfigureMaintenanceEndpoints(this WebApplication app)
    {
        var maintenanceGroup = app.MapGroup("/api/v1/maintenances")
            .WithTags("Maintenances")
            .WithOpenApi();

        // GET /api/v1/maintenances - Version simplifiée pour test
        maintenanceGroup.MapGet("/", () =>
        {
            return Results.Ok(new { message = "API Maintenance opérationnelle", timestamp = DateTime.UtcNow });
        })
        .WithName("GetMaintenances")
        .WithSummary("Récupère la liste des maintenances")
        .WithDescription("Endpoint de test - retourne un message de confirmation")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
