using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Commands.Maintenances;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Interface pour le service client API de maintenance
/// </summary>
public interface IMaintenanceApiService : IApiClientService<MaintenanceDto, CreateMaintenanceCommand, UpdateMaintenanceCommand>
{
    Task<IEnumerable<MaintenanceDto>?> GetMaintenancesByVehicleAsync(int vehicleId);
    Task<IEnumerable<MaintenanceDto>?> GetUpcomingMaintenancesAsync(int daysAhead = 30);
}
