namespace Laroche.FleetManager.API.Endpoints;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        // GET /api/v1/drivers/all
        driverGroup.MapGet("/all", async ([FromServices] IDriverService driverService) =>
        {
            var result = await driverService.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllDrivers")
        .WithSummary("Récupère tous les conducteurs")
        .Produces<IEnumerable<DriverDto>>(StatusCodes.Status200OK);

        // GET /api/v1/drivers/{id}/details
        driverGroup.MapGet("/{id:int}/details", async ([FromServices] IDriverService driverService, int id) =>
        {
            var result = await driverService.GetDriverByIdAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetDriverDetails")
        .WithSummary("Récupère les détails d'un conducteur")
        .Produces<DriverDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/drivers/create
        driverGroup.MapPost("/create", async ([FromServices] IDriverService driverService, [FromBody] CreateDriverDto dto) =>
        {
            var result = await driverService.CreateDriverAsync(dto);
            return Results.Created($"/api/v1/drivers/{result.Id}", result);
        })
        .WithName("CreateDriver")
        .WithSummary("Crée un nouveau conducteur")
        .Produces<DriverDto>(StatusCodes.Status201Created);

        // PUT /api/v1/drivers/{id}
        driverGroup.MapPut("/{id:int}", async ([FromServices] IDriverService driverService, int id, [FromBody] UpdateDriverDto dto) =>
        {
            var result = await driverService.UpdateDriverAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateDriver")
        .WithSummary("Met à jour un conducteur")
        .Produces<DriverDto>(StatusCodes.Status200OK);

        // DELETE /api/v1/drivers/{id}
        driverGroup.MapDelete("/{id:int}", async ([FromServices] IDriverService driverService, int id) =>
        {
            await driverService.DeleteDriverAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteDriver")
        .WithSummary("Supprime un conducteur")
        .Produces(StatusCodes.Status204NoContent);

        // GET /api/v1/drivers/count
        driverGroup.MapGet("/count", async ([FromServices] IDriverService driverService) =>
        {
            var count = await driverService.GetCountAsync();
            return Results.Ok(new { count, timestamp = DateTime.UtcNow });
        })
        .WithName("GetDriversCount")
        .WithSummary("Récupère le nombre total de conducteurs")
        .WithDescription("Retourne le nombre total de conducteurs dans le système")
        .Produces<int>(StatusCodes.Status200OK);


        return app;
    }
}
