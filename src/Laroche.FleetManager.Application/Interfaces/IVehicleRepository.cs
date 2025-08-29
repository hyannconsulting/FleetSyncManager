using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.Interfaces;

// Alias pour simplifier l'usage
using VehicleStatus = Laroche.FleetManager.Domain.Enums.VehicleStatusEnums;

/// <summary>
/// Interface pour le repository des véhicules
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Récupère tous les véhicules avec pagination et filtres
    /// </summary>
    Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? brand = null,
        VehicleStatus? status = null,
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
}
