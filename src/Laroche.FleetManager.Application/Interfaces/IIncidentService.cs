using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Incidents;

namespace Laroche.FleetManager.Application.Interfaces;

public interface IIncidentService
{
    /// <summary>
    /// Retrieves a paginated list of incidents based on the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters used to filter and paginate the incidents.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{T}"/> of
    /// <see cref="IncidentDto"/> objects matching the query.</returns>
    Task<PagedResult<IncidentDto>> GetPagedAsync(GetAllIncidentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les incidents.
    /// </summary>
    Task<IEnumerable<IncidentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un incident par son identifiant.
    /// </summary>
    Task<IncidentDto?> GetIncidentByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Crée un nouvel incident.
    /// </summary>
    Task<IncidentDto> CreateIncidentAsync(CreateIncidentDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un incident existant.
    /// </summary>
    Task<IncidentDto> UpdateIncidentAsync(int id, UpdateIncidentDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un incident par son identifiant.
    /// </summary>
    Task DeleteIncidentAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marque un incident comme résolu.
    /// </summary>
    Task MarkAsResolvedAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère le nombre total d'incidents.
    /// </summary>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

}
