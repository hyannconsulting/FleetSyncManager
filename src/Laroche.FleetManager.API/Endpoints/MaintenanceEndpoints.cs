namespace Laroche.FleetManager.API.Endpoints;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        // GET /api/v1/maintenances/all
        maintenanceGroup.MapGet("/all", async ([FromServices] IMaintenanceService maintenanceService) =>
        {
            var result = await maintenanceService.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllMaintenances")
        .WithSummary("Récupère toutes les maintenances")
        .Produces<IEnumerable<MaintenanceDto>>(StatusCodes.Status200OK);

        // GET /api/v1/maintenances/{id}/details
        maintenanceGroup.MapGet("/{id:int}/details", async ([FromServices] IMaintenanceService maintenanceService, int id) =>
        {
            var result = await maintenanceService.GetMaintenanceByIdAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetMaintenanceDetails")
        .WithSummary("Récupère les détails d'une maintenance")
        .Produces<MaintenanceDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/maintenances/create
        maintenanceGroup.MapPost("/create", async ([FromServices] IMaintenanceService maintenanceService, [FromBody] CreateMaintenanceDto dto) =>
        {
            var result = await maintenanceService.CreateMaintenanceAsync(dto);
            return Results.Created($"/api/v1/maintenances/{result.Id}", result);
        })
        .WithName("CreateMaintenance")
        .WithSummary("Crée une nouvelle maintenance")
        .Produces<MaintenanceDto>(StatusCodes.Status201Created);

        // PUT /api/v1/maintenances/{id}
        maintenanceGroup.MapPut("/{id:int}", async ([FromServices] IMaintenanceService maintenanceService, int id, [FromBody] UpdateMaintenanceDto dto) =>
        {
            var result = await maintenanceService.UpdateMaintenanceAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateMaintenance")
        .WithSummary("Met à jour une maintenance")
        .Produces<MaintenanceDto>(StatusCodes.Status200OK);

        // DELETE /api/v1/maintenances/{id}
        maintenanceGroup.MapDelete("/{id:int}", async ([FromServices] IMaintenanceService maintenanceService, int id) =>
        {
            await maintenanceService.DeleteMaintenanceAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteMaintenance")
        .WithSummary("Supprime une maintenance")
        .Produces(StatusCodes.Status204NoContent);

        // GET /api/v1/maintenances/count
        maintenanceGroup.MapGet("/count", async ([FromServices] IMaintenanceService maintenanceService) =>
        {
            var count = await maintenanceService.GetCountAsync();
            return Results.Ok(new { count, timestamp = DateTime.UtcNow });
        })
        .WithName("GetMaintenancesCount")
        .WithSummary("Récupère le nombre total de maintenances")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
