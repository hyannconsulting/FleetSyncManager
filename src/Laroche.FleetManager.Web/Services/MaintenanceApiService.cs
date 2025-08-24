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

/// <summary>
/// Service client API pour la maintenance
/// </summary>
public class MaintenanceApiService : BaseApiClientService<MaintenanceDto, CreateMaintenanceCommand, UpdateMaintenanceCommand>, IMaintenanceApiService
{
    protected override string ApiEndpoint => "/api/v1/maintenances";

    public MaintenanceApiService(HttpClient httpClient, ILogger<MaintenanceApiService> logger) 
        : base(httpClient, logger)
    {
    }

    /// <summary>
    /// Récupère les maintenances d'un véhicule
    /// </summary>
    public async Task<IEnumerable<MaintenanceDto>?> GetMaintenancesByVehicleAsync(int vehicleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/vehicle/{vehicleId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<MaintenanceDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des maintenances du véhicule {VehicleId}: {StatusCode}", vehicleId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des maintenances du véhicule {VehicleId}", vehicleId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les maintenances à venir
    /// </summary>
    public async Task<IEnumerable<MaintenanceDto>?> GetUpcomingMaintenancesAsync(int daysAhead = 30)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/upcoming?daysAhead={daysAhead}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<MaintenanceDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des maintenances à venir: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des maintenances à venir");
            return null;
        }
    }
}
