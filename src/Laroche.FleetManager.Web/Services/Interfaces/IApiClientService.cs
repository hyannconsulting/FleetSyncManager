using Laroche.FleetManager.Application.Common;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Interface pour les services API client
/// </summary>
public interface IApiClientService<TDto, TCreateCommand, TUpdateCommand>
    where TDto : class
    where TCreateCommand : class
    where TUpdateCommand : class
{
    Task<PagedResult<TDto>?> GetAllAsync(int page = 1, int pageSize = 10, string? searchTerm = null);
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto?> CreateAsync(TCreateCommand command);
    Task<TDto?> UpdateAsync(int id, TUpdateCommand command);
    Task<bool> DeleteAsync(int id);
}
