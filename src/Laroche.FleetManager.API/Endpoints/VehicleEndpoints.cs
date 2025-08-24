namespace Laroche.FleetManager.API.Endpoints;

/// <summary>
/// Configuration des endpoints pour la gestion des véhicules
/// </summary>
public static class VehicleEndpoints
{
    /// <summary>
    /// Configure les endpoints pour les véhicules
    /// </summary>
    public static WebApplication ConfigureVehicleEndpoints(this WebApplication app)
    {
        var vehicleGroup = app.MapGroup("/api/v1/vehicles")
            .WithTags("Vehicles")
            .WithOpenApi();

        // GET /api/v1/vehicles - Version simplifiée pour test
        vehicleGroup.MapGet("/", () =>
        {
            return Results.Ok(new { message = "API Véhicules opérationnelle", timestamp = DateTime.UtcNow });
        })
        .WithName("GetVehicles")
        .WithSummary("Récupère la liste des véhicules")
        .WithDescription("Endpoint de test - retourne un message de confirmation")
        .Produces<object>(StatusCodes.Status200OK);

        // GET /api/v1/vehicles/{id} - Version simplifiée
        vehicleGroup.MapGet("/{id:int}", (int id) =>
        {
            return Results.Ok(new { id = id, message = $"Véhicule {id} trouvé", timestamp = DateTime.UtcNow });
        })
        .WithName("GetVehicleById")
        .WithSummary("Récupère un véhicule par son ID")
        .WithDescription("Endpoint de test - retourne les informations basiques d'un véhicule")
        .Produces<object>(StatusCodes.Status200OK);

        // POST /api/v1/vehicles - Version simplifiée
        vehicleGroup.MapPost("/", (object vehicle) =>
        {
            return Results.Ok(new { message = "Véhicule créé avec succès", data = vehicle, timestamp = DateTime.UtcNow });
        })
        .WithName("CreateVehicle")
        .WithSummary("Crée un nouveau véhicule")
        .WithDescription("Endpoint de test - simule la création d'un véhicule")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
