using Laroche.FleetManager.Application.Commands.Maintenances;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Interface pour le service client API de maintenance
/// </summary>
public interface IMaintenanceApiService :
    IApiClientService<MaintenanceDto, CreateMaintenanceCommand, UpdateMaintenanceCommand>
{
    Task<IEnumerable<MaintenanceDto>?> GetMaintenancesByVehicleAsync(int vehicleId);
    Task<IEnumerable<MaintenanceDto>?> GetUpcomingMaintenancesAsync(int daysAhead = 30);

    /// <summary>
    /// Récupère le nombre total de maintenances.
    /// </summary>
    Task<int> GetMaintenanceCountAsync();

    /// <summary>
    /// Récupère une page de maintenances paginée.
    /// </summary>
    /// <param name="page">Numéro de page</param>
    /// <param name="pageSize">Taille de page</param>
    /// <returns>Résultat paginé</returns>
    Task<PagedResult<MaintenanceDto>?> GetPagedAsync(int page, int pageSize);

    /// <summary>
    /// Marks a maintenance task as completed with the specified completion details.
    /// </summary>
    /// <remarks>This method updates the status of the specified maintenance task to "Completed" and records the
    /// provided completion details. Ensure that the <paramref name="maintenanceId"/> corresponds to a valid maintenance
    /// task before calling this method.</remarks>
    /// <param name="maintenanceId">The unique identifier of the maintenance task to complete.</param>
    /// <param name="completedDate">The date and time when the maintenance task was completed.</param>
    /// <param name="actualCost">The actual cost incurred for completing the maintenance task. Must be a non-negative value.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the maintenance task
    /// was successfully marked as completed; otherwise, <see langword="false"/>.</returns>
    Task<bool> CompleteMaintenanceAsync(int maintenanceId, DateTime completedDate, decimal actualCost);

    /// <summary>
    /// Récupère les maintenances terminées.
    /// </summary>
    /// <param name="vehicleId">ID du véhicule (optionnel)</param>
    /// <param name="fromDate">Date de début pour filtrer (optionnel)</param>
    /// <param name="toDate">Date de fin pour filtrer (optionnel)</param>
    /// <returns>Liste des maintenances terminées</returns>
    Task<IEnumerable<MaintenanceDto>?> GetCompletedMaintenancesAsync(int? vehicleId = null, DateTime? fromDate = null, DateTime? toDate = null);
}
