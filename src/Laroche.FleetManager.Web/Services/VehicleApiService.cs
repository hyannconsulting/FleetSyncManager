using Laroche.FleetManager.Application.Commands.Vehicles;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Laroche.FleetManager.Web.Services.Interfaces;

namespace Laroche.FleetManager.Web.Services;

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

    /// <summary>
    /// Récupère les véhicules avec pagination et filtres
    /// </summary>
    public async Task<PagedResult<VehicleDto>?> GetPagedAsync(GetVehiclesQuery query)
    {
        try
        {
            var queryParams = new List<string>();

            // Paramètres de pagination
            queryParams.Add($"page={query.Page}");
            queryParams.Add($"pageSize={query.PageSize}");

            // Paramètres de recherche et filtres
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(query.SearchTerm)}");

            if (!string.IsNullOrWhiteSpace(query.Brand))
                queryParams.Add($"brand={Uri.EscapeDataString(query.Brand)}");

            if (!string.IsNullOrWhiteSpace(query.Status))
                queryParams.Add($"status={Uri.EscapeDataString(query.Status)}");

            if (!string.IsNullOrWhiteSpace(query.FuelType))
                queryParams.Add($"fuelType={Uri.EscapeDataString(query.FuelType)}");

            // Paramètres de tri
            if (!string.IsNullOrWhiteSpace(query.SortBy))
                queryParams.Add($"sortBy={Uri.EscapeDataString(query.SortBy)}");

            if (!string.IsNullOrWhiteSpace(query.SortDirection))
                queryParams.Add($"sortDirection={Uri.EscapeDataString(query.SortDirection)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{ApiEndpoint}{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<PagedResult<VehicleDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des véhicules paginés: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des véhicules paginés");
            return null;
        }
    }

    /// <summary>
    /// Récupère le nombre total de véhicules
    /// </summary>
    public async Task<int?> GetTotalCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/count");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération du nombre total de véhicules: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du nombre total de véhicules");
            return null;
        }
    }
}
