using Laroche.FleetManager.Application.Common;
using System.Text.Json;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Service de base pour les clients API
/// </summary>
public abstract class BaseApiClientService<TDto, TCreateCommand, TUpdateCommand>
    : IApiClientService<TDto, TCreateCommand, TUpdateCommand>
    where TDto : class
    where TCreateCommand : class
    where TUpdateCommand : class
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger<BaseApiClientService<TDto, TCreateCommand, TUpdateCommand>> _logger;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected abstract string ApiEndpoint { get; }

    protected BaseApiClientService(
        HttpClient httpClient,
        ILogger<BaseApiClientService<TDto, TCreateCommand, TUpdateCommand>> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Récupère tous les éléments avec pagination
    /// </summary>
    public virtual async Task<PagedResult<TDto>?> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null)
    {
        try
        {
            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{ApiEndpoint}?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PagedResult<TDto>>(json, _jsonOptions);
            }

            _logger.LogWarning("Échec de récupération des données depuis l'API: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des données depuis l'API");
            return null;
        }
    }

    /// <summary>
    /// Récupère un élément par ID
    /// </summary>
    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TDto>(json, _jsonOptions);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Élément avec ID {Id} non trouvé", id);
                return null;
            }

            _logger.LogWarning("Échec de récupération de l'élément {Id}: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'élément {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Crée un nouvel élément
    /// </summary>
    public virtual async Task<TDto?> CreateAsync(TCreateCommand command)
    {
        try
        {
            var json = JsonSerializer.Serialize(command, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TDto>(responseJson, _jsonOptions);
            }

            _logger.LogWarning("Échec de création: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création");
            return null;
        }
    }

    /// <summary>
    /// Met à jour un élément existant
    /// </summary>
    public virtual async Task<TDto?> UpdateAsync(int id, TUpdateCommand command)
    {
        try
        {
            var json = JsonSerializer.Serialize(command, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ApiEndpoint}/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TDto>(responseJson, _jsonOptions);
            }

            _logger.LogWarning("Échec de mise à jour de l'élément {Id}: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'élément {Id}", id);
            return null;
        }
    }

    /// <summary>
    /// Supprime un élément
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}/{id}");

            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            _logger.LogWarning("Échec de suppression de l'élément {Id}: {StatusCode}", id, response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de l'élément {Id}", id);
            return false;
        }
    }
}
