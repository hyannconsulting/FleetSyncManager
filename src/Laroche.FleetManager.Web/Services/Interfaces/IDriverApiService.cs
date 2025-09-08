using Laroche.FleetManager.Application.Commands.Drivers;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Interface pour le service client API des conducteurs
/// </summary>
public interface IDriverApiService : IApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>
{
    /// <summary>
    /// Asynchronously retrieves the total number of drivers.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of drivers.</returns>
    Task<int> GetDriverCountAsync();

    /// <summary>
    /// Récupère une page de conducteurs paginée.
    /// </summary>
    /// <param name="page">Numéro de page</param>
    /// <param name="pageSize">Taille de page</param>
    /// <returns>Résultat paginé</returns>
    Task<PagedResult<DriverDto>?> GetPagedAsync(int page, int pageSize);
}
