using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Commands.Vehicles;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Interface pour le service client API des véhicules
/// </summary>
public interface IVehicleApiService : IApiClientService<VehicleDto, CreateVehicleCommand, UpdateVehicleCommand>
{
    Task<IEnumerable<VehicleDto>?> GetVehiclesByDriverAsync(int driverId, bool includeInactive = false);
    Task<IEnumerable<VehicleDto>?> GetVehiclesNeedingMaintenanceAsync(int daysAhead = 30, bool includeOverdue = true);
}
