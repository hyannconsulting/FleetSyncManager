namespace Laroche.FleetManager.API.Endpoints;

/// <summary>
/// Configuration des endpoints pour la gestion des incidents
/// </summary>
public static class IncidentEndpoints
{
    /// <summary>
    /// Configure les endpoints pour les incidents
    /// </summary>
    public static WebApplication ConfigureIncidentEndpoints(this WebApplication app)
    {
        var incidentGroup = app.MapGroup("/api/v1/incidents")
            .WithTags("Incidents")
            .WithOpenApi();

        // GET /api/v1/incidents - Version simplifiée pour test
        incidentGroup.MapGet("/", () =>
        {
            return Results.Ok(new { message = "API Incidents opérationnelle", timestamp = DateTime.UtcNow });
        })
        .WithName("GetIncidents")
        .WithSummary("Récupère la liste des incidents")
        .WithDescription("Endpoint de test - retourne un message de confirmation")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
