using Laroche.FleetManager.Application.Commands.Incidents;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Incidents;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Service client API pour les incidents
/// </summary>
public class IncidentApiService(HttpClient httpClient, ILogger<IncidentApiService> logger) :
    BaseApiClientService<IncidentDto, CreateIncidentCommand, UpdateIncidentCommand>(httpClient, logger), IIncidentApiService
{
    protected override string ApiEndpoint => "/api/v1/incidents";

    /// <summary>
    /// Récupère les incidents d'un véhicule
    /// </summary>
    public async Task<IEnumerable<IncidentDto>?> GetIncidentsByVehicleAsync(int vehicleId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/vehicle/{vehicleId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<IncidentDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des incidents du véhicule {VehicleId}: {StatusCode}", vehicleId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des incidents du véhicule {VehicleId}", vehicleId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les incidents d'un conducteur
    /// </summary>
    public async Task<IEnumerable<IncidentDto>?> GetIncidentsByDriverAsync(int driverId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/driver/{driverId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<IncidentDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des incidents du conducteur {DriverId}: {StatusCode}", driverId, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des incidents du conducteur {DriverId}", driverId);
            return null;
        }
    }

    /// <summary>
    /// Récupère les incidents en attente
    /// </summary>
    public async Task<IEnumerable<IncidentDto>?> GetPendingIncidentsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/pending");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<IncidentDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des incidents en attente: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des incidents en attente");
            return null;
        }
    }

    public async Task<PagedResult<IncidentDto>?> GetPagedAsync(GetAllIncidentsQuery query)
    {
        try
        {
            var queryParams = new List<string>();

            if (query.Page > 0)
                queryParams.Add($"page={query.Page}");

            if (query.PageSize > 0)
                queryParams.Add($"pageSize={query.PageSize}");

            if (!string.IsNullOrWhiteSpace(query.IncidentType))
                queryParams.Add($"incidentType={Uri.EscapeDataString(query.IncidentType)}");

            if (query.VehicleId.HasValue)
                queryParams.Add($"vehicleId={query.VehicleId.Value}");

            if (query.DriverId.HasValue)
                queryParams.Add($"driverId={query.DriverId.Value}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
            var response = await _httpClient.GetAsync($"{ApiEndpoint}{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<PagedResult<IncidentDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des incidents paginés: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des incidents paginés");
            return null;
        }
    }

    public async Task<bool> MarkAsResolvedAsync(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"{ApiEndpoint}/{id}/resolve", null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Échec de résolution de l'incident {IncidentId}: {StatusCode}", id, response.StatusCode);
                return false;
            }
            else
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la résolution de l'incident {IncidentId}", id);
            throw;
        }
    }

    public async Task<int> GetIncidentCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/count");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<int>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération du nombre d'incidents: {StatusCode}", response.StatusCode);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du nombre d'incidents");
            return 0;
        }
    }

    public async Task<IEnumerable<IncidentDto>?> GetRecentOpenIncidentsAsync(int count = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/recent-open?count={count}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<IncidentDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des incidents ouverts récents: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des incidents ouverts récents");
            return null;
        }
    }
}
