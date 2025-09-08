using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.Interfaces;

/// <summary>
/// Interface pour le repository des véhicules (CRUD + pagination)
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Récupère les véhicules paginés via un objet de requête
    /// </summary>
    Task<PagedResult<VehicleDto>> GetPagedAsync(GetVehiclesQuery query);

    /// <summary>
    /// Récupère tous les véhicules avec pagination et filtres
    /// </summary>
    Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? brand = null,
        VehicleStatusEnums? status = null,
        FuelType? fuelType = null,
        string sortBy = "LicensePlate",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un véhicule par ID
    /// </summary>
    Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ajoute un nouveau véhicule
    /// </summary>
    Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un véhicule
    /// </summary>
    Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un véhicule (soft delete)
    /// </summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vérifie si une plaque d'immatriculation existe déjà
    /// </summary>
    Task<bool> LicensePlateExistsAsync(string licensePlate, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the count of items.
    /// </summary>
    /// <remarks>This method is designed to be non-blocking and supports cancellation through the provided
    /// <paramref name="cancellationToken"/>.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of items as an integer.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
