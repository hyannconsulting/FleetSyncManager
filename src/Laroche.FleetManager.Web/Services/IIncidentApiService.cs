using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Commands.Incidents;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Interface pour le service client API des incidents
/// </summary>
public interface IIncidentApiService : IApiClientService<IncidentDto, CreateIncidentCommand, UpdateIncidentCommand>
{
    Task<IEnumerable<IncidentDto>?> GetIncidentsByVehicleAsync(int vehicleId);
    Task<IEnumerable<IncidentDto>?> GetIncidentsByDriverAsync(int driverId);
    Task<IEnumerable<IncidentDto>?> GetPendingIncidentsAsync();
}
