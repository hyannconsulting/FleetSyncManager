
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Microsoft.AspNetCore.Mvc;
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

        // GET /api/v1/vehicles/paged
        vehicleGroup.MapGet("/paged", async (
            [FromServices] IVehicleService vehicleService,
            [AsParameters] GetVehiclesQuery query) =>
        {
            var result = await vehicleService.GetPagedAsync(query);
            return Results.Ok(result);
        })
        .WithName("GetPagedVehicles")
        .WithSummary("Récupère les véhicules paginés")
        .Produces<PagedResult<VehicleDto>>(StatusCodes.Status200OK);
        // GET /api/v1/vehicles (réel)
        vehicleGroup.MapGet("/all", async ([FromServices] IVehicleService vehicleService) =>
        {
            var result = await vehicleService.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllVehicles")
        .WithSummary("Récupère tous les véhicules")
        .Produces<IEnumerable<VehicleDto>>(StatusCodes.Status200OK);

        // GET /api/v1/vehicles/{id} (réel)
        vehicleGroup.MapGet("/{id:int}/details", async ([FromServices] IVehicleService vehicleService, int id) =>
        {
            var result = await vehicleService.GetVehicleByIdAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetVehicleDetails")
        .WithSummary("Récupère les détails d'un véhicule")
        .Produces<VehicleDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/vehicles (réel)
        vehicleGroup.MapPost("/create", async ([FromServices] IVehicleService vehicleService, [FromBody] CreateVehicleDto dto) =>
        {
            var result = await vehicleService.CreateVehicleAsync(dto);
            return Results.Created($"/api/v1/vehicles/{result.Id}", result);
        })
        .WithName("CreateVehicleReal")
        .WithSummary("Crée un nouveau véhicule")
        .Produces<VehicleDto>(StatusCodes.Status201Created);

        // PUT /api/v1/vehicles/{id}
        vehicleGroup.MapPut("/{id:int}", async ([FromServices] IVehicleService vehicleService, int id, [FromBody] UpdateVehicleDto dto) =>
        {
            var result = await vehicleService.UpdateVehicleAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateVehicle")
        .WithSummary("Met à jour un véhicule")
        .Produces<VehicleDto>(StatusCodes.Status200OK);

        // DELETE /api/v1/vehicles/{id}
        vehicleGroup.MapDelete("/{id:int}", async ([FromServices] IVehicleService vehicleService, int id) =>
        {
            await vehicleService.DeleteVehicleAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteVehicle")
        .WithSummary("Supprime un véhicule")
        .Produces(StatusCodes.Status204NoContent);

        // GET /api/v1/vehicles/count
        vehicleGroup.MapGet("/count", async ([FromServices] IVehicleService vehicleService) =>
        {
            var count = await vehicleService.GetCountAsync();
            return Results.Ok(new { count, timestamp = DateTime.UtcNow });
        })
        .WithName("GetVehiclesCount")
        .WithSummary("Récupère le nombre total de véhicules")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }

}