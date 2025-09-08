
using Laroche.FleetManager.Application.Commands.Incidents;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Incidents;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Interface pour le service client API des incidents
/// </summary>
public interface IIncidentApiService : IApiClientService<IncidentDto, CreateIncidentCommand, UpdateIncidentCommand>
{
    Task<IEnumerable<IncidentDto>?> GetIncidentsByVehicleAsync(int vehicleId);
    Task<IEnumerable<IncidentDto>?> GetIncidentsByDriverAsync(int driverId);
    Task<IEnumerable<IncidentDto>?> GetPendingIncidentsAsync();

    // Pagination et résolution
    Task<PagedResult<IncidentDto>?> GetPagedAsync(GetAllIncidentsQuery query);

    /// <summary>
    /// Marks the item with the specified identifier as resolved.
    /// </summary>
    /// <remarks>This method performs the operation asynchronously. Ensure that the item with the specified
    /// identifier exists before calling this method.</remarks>
    /// <param name="id">The unique identifier of the item to mark as resolved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the item was
    /// successfully marked as resolved; otherwise, <see langword="false"/>.</returns>
    Task<bool> MarkAsResolvedAsync(int id);

    /// <summary>
    /// Récupère le nombre total d'incidents.
    /// </summary>
    Task<int> GetIncidentCountAsync();

    /// <summary>
    /// Récupère les incidents ouverts les plus récents (limite n).
    /// </summary>
    Task<IEnumerable<IncidentDto>?> GetRecentOpenIncidentsAsync(int count = 10);
}
