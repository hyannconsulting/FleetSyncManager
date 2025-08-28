using Laroche.FleetManager.Application.Commands.Incidents;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Service client API pour les incidents
/// </summary>
public class IncidentApiService : BaseApiClientService<IncidentDto, CreateIncidentCommand, UpdateIncidentCommand>, IIncidentApiService
{
    protected override string ApiEndpoint => "/api/v1/incidents";

    public IncidentApiService(HttpClient httpClient, ILogger<IncidentApiService> logger)
        : base(httpClient, logger)
    {
    }

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
}
