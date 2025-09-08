// using Laroche.FleetManager.Application.DTOs; // déjà importé plus bas
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Vehicles;

namespace Laroche.FleetManager.Application.Interfaces;

    /// <summary>
    /// Interface pour la gestion des véhicules (CRUD, pagination, etc.)
    /// </summary>
    public interface IVehicleService
{
    /// <summary>
    /// Récupère les véhicules paginés selon les critères de recherche.
    /// </summary>
    Task<PagedResult<VehicleDto>> GetPagedAsync(GetVehiclesQuery query);
    /// <summary>
    /// Récupère tous les véhicules.
    /// </summary>
    Task<IEnumerable<VehicleDto>> GetAllAsync();

    /// <summary>
    /// Récupère un véhicule par son identifiant.
    /// </summary>
    Task<VehicleDto?> GetVehicleByIdAsync(int id);

    /// <summary>
    /// Crée un nouveau véhicule.
    /// </summary>
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto);

    /// <summary>
    /// Met à jour un véhicule existant.
    /// </summary>
    Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto);

    /// <summary>
    /// Supprime un véhicule par son identifiant.
    /// </summary>
    Task DeleteVehicleAsync(int id);

    /// <summary>
    /// Récupère le nombre total de véhicules.
    /// </summary>
    Task<int> GetCountAsync(System.Threading.CancellationToken cancellationToken = default);
}
