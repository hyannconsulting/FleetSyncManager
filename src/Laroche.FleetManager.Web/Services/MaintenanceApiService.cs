using Laroche.FleetManager.Application.Commands.Maintenances;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Web.Services.Interfaces;

namespace Laroche.FleetManager.Web.Services;

using Laroche.FleetManager.Application.Common;

/// <summary>
/// Service client API pour la maintenance
/// </summary>
public class MaintenanceApiService(HttpClient httpClient, ILogger<MaintenanceApiService> logger) :
    BaseApiClientService<MaintenanceDto, CreateMaintenanceCommand, UpdateMaintenanceCommand>(httpClient, logger),
    IMaintenanceApiService
{
    protected override string ApiEndpoint => "/api/v1/maintenances";


    /// <summary>
    /// Récupère le nombre total de maintenances.
    /// </summary>
    public async Task<int> GetMaintenanceCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/count");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                if (int.TryParse(json, out var count))
                    return count;
            }
            _logger.LogWarning("Échec de récupération du count des maintenances: {StatusCode}", response.StatusCode);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du count des maintenances");
            return 0;
        }
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

    /// <summary>
    /// Récupère les maintenances terminées avec filtres optionnels
    /// </summary>
    public async Task<IEnumerable<MaintenanceDto>?> GetCompletedMaintenancesAsync(int? vehicleId, DateTime? fromDate, DateTime? toDate)
    {
        try
        {
            var queryParams = new List<string>();

            if (vehicleId.HasValue)
                queryParams.Add($"vehicleId={vehicleId.Value}");

            if (fromDate.HasValue)
                queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");

            if (toDate.HasValue)
                queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

            var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/completed{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<MaintenanceDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des maintenances terminées: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des maintenances terminées");
            return null;
        }
    }

    /// <summary>
    /// Marque une maintenance comme terminée
    /// </summary>
    /// <param name="maintenanceId">ID de la maintenance</param>
    /// <param name="completedDate">Date de complétion</param>
    /// <param name="actualCost">Coût réel</param>
    /// <returns>True si succès, false sinon</returns>
    public async Task<bool> CompleteMaintenanceAsync(int maintenanceId, DateTime completedDate, decimal actualCost)
    {
        try
        {
            var completeData = new
            {
                CompletedDate = completedDate,
                ActualCost = actualCost,
                Status = "Completed"
            };

            var json = System.Text.Json.JsonSerializer.Serialize(completeData, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ApiEndpoint}/{maintenanceId}/complete", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Maintenance {MaintenanceId} marquée comme terminée avec succès", maintenanceId);
                return true;
            }

            _logger.LogWarning("Échec de la complétion de la maintenance {MaintenanceId}: {StatusCode}", maintenanceId, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la complétion de la maintenance {MaintenanceId}", maintenanceId);
            return false;
        }
    }

    /// <summary>
    /// Récupère une page de maintenances
    /// </summary>
    public async Task<PagedResult<MaintenanceDto>?> GetPagedAsync(int page, int pageSize)
    {
        return await GetAllAsync(page, pageSize);
    }
}
