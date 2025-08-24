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

/// <summary>
/// Service client API pour les véhicules
/// </summary>
public class VehicleApiService : BaseApiClientService<VehicleDto, CreateVehicleCommand, UpdateVehicleCommand>, IVehicleApiService
{
    protected override string ApiEndpoint => "/api/v1/vehicles";

    public VehicleApiService(HttpClient httpClient, ILogger<VehicleApiService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Récupère les véhicules assignés à un conducteur
    /// </summary>
    public async Task<IEnumerable<VehicleDto>?> GetVehiclesByDriverAsync(int driverId, bool includeInactive = false)
    {
        try
        {
            var queryParams = includeInactive ? "?includeInactive=true" : "";
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/driver/{driverId}{queryParams}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<VehicleDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des véhicules du conducteur {DriverId}: {StatusCode}", driverId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des véhicules du conducteur {DriverId}", driverId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les véhicules nécessitant une maintenance
    /// </summary>
    public async Task<IEnumerable<VehicleDto>?> GetVehiclesNeedingMaintenanceAsync(int daysAhead = 30, bool includeOverdue = true)
    {
        try
        {
            var queryParams = $"?daysAhead={daysAhead}&includeOverdue={includeOverdue}";
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/maintenance-needed{queryParams}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<VehicleDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des véhicules nécessitant une maintenance: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des véhicules nécessitant une maintenance");
            return null;
        }
    }
}
