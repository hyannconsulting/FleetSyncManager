
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.Queries.Incidents;
namespace Laroche.FleetManager.API.Endpoints;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        // GET /api/v1/incidents/paged
        incidentGroup.MapGet("/paged", async (
            [FromServices] IIncidentService incidentService,
            [AsParameters] GetAllIncidentsQuery query) =>
        {
            var result = await incidentService.GetPagedAsync(query);
            return Results.Ok(result);
        })
        .WithName("GetPagedIncidents")
        .WithSummary("Récupère les incidents paginés")
        .Produces<PagedResult<IncidentDto>>(StatusCodes.Status200OK);

        // POST /api/v1/incidents/{id}/resolve
        incidentGroup.MapPost("/{id:int}/resolve", async (
            [FromServices] IIncidentService incidentService,
            int id) =>
        {
            await incidentService.MarkAsResolvedAsync(id);
            return Results.NoContent();
        })
        .WithName("ResolveIncident")
        .WithSummary("Marque un incident comme résolu")
        .Produces(StatusCodes.Status204NoContent);
        // GET /api/v1/incidents/all
        incidentGroup.MapGet("/all", async ([FromServices] IIncidentService incidentService) =>
        {
            var result = await incidentService.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllIncidents")
        .WithSummary("Récupère tous les incidents")
        .Produces<IEnumerable<IncidentDto>>(StatusCodes.Status200OK);

        // GET /api/v1/incidents/{id}/details
        incidentGroup.MapGet("/{id:int}/details", async ([FromServices] IIncidentService incidentService, int id) =>
        {
            var result = await incidentService.GetIncidentByIdAsync(id);
            return result is not null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetIncidentDetails")
        .WithSummary("Récupère les détails d'un incident")
        .Produces<IncidentDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/incidents/create
        incidentGroup.MapPost("/create", async ([FromServices] IIncidentService incidentService, [FromBody] CreateIncidentDto dto) =>
        {
            var result = await incidentService.CreateIncidentAsync(dto);
            return Results.Created($"/api/v1/incidents/{result.Id}", result);
        })
        .WithName("CreateIncident")
        .WithSummary("Crée un nouvel incident")
        .Produces<IncidentDto>(StatusCodes.Status201Created);

        // PUT /api/v1/incidents/{id}
        incidentGroup.MapPut("/{id:int}", async ([FromServices] IIncidentService incidentService, int id, [FromBody] UpdateIncidentDto dto) =>
        {
            var result = await incidentService.UpdateIncidentAsync(id, dto);
            return Results.Ok(result);
        })
        .WithName("UpdateIncident")
        .WithSummary("Met à jour un incident")
        .Produces<IncidentDto>(StatusCodes.Status200OK);

        // DELETE /api/v1/incidents/{id}
        incidentGroup.MapDelete("/{id:int}", async ([FromServices] IIncidentService incidentService, int id) =>
        {
            await incidentService.DeleteIncidentAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteIncident")
        .WithSummary("Supprime un incident")
        .Produces(StatusCodes.Status204NoContent);

        // GET /api/v1/incidents/count
        incidentGroup.MapGet("/count", async ([FromServices] IIncidentService incidentService) =>
        {
            var count = await incidentService.GetCountAsync();
            return Results.Ok(new { count, timestamp = DateTime.UtcNow });
        })
        .WithName("GetIncidentsCount")
        .WithSummary("Récupère le nombre total d'incidents")
        .Produces<int>(StatusCodes.Status200OK);


        return app;
    }
}
