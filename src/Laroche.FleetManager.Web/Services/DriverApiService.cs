using Laroche.FleetManager.Application.Commands.Drivers;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Web.Services.Interfaces;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Provides functionality to interact with the Driver API, enabling operations such as retrieving, creating, and
/// updating driver data.
/// </summary>
/// <remarks>This service is designed to communicate with the Driver API at the endpoint <c>/api/v1/drivers</c>. 
/// It leverages HTTP client functionality to perform API requests and supports logging for diagnostic
/// purposes.</remarks>
public class DriverApiService(HttpClient httpClient, ILogger<DriverApiService> logger) :
    BaseApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>(httpClient, logger), IDriverApiService
{
    protected override string ApiEndpoint => "/api/v1/drivers";

    public async Task<int> GetDriverCountAsync()
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
            _logger.LogWarning("�chec de r�cup�ration du count des drivers: {StatusCode}", response.StatusCode);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la r�cup�ration du count des drivers");
            return 0;
        }
    }

    /// <summary>
    /// Récupère une page de conducteurs paginée.
    /// </summary>
    /// <param name="page">Numéro de page</param>
    /// <param name="pageSize">Taille de page</param>
    /// <returns>Résultat paginé</returns>
    public async Task<PagedResult<DriverDto>?> GetPagedAsync(int page, int pageSize)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/paged?page={page}&pageSize={pageSize}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<PagedResult<DriverDto>>(json, _jsonOptions);
            }
            _logger.LogWarning("Échec de récupération des conducteurs paginés: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des conducteurs paginés");
            return null;
        }
    }
}
