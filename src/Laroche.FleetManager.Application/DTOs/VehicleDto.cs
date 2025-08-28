using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.DTOs;

/// <summary>
/// Vehicle Data Transfer Object
/// </summary>
public class VehicleDto
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Vehicle license plate
    /// </summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle VIN number
    /// </summary>
    public string? Vin { get; set; }

    /// <summary>
    /// Vehicle brand
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle model
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle year
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Vehicle fuel type
    /// </summary>
    public FuelType FuelType { get; set; }

    /// <summary>
    /// Current mileage
    /// </summary>
    public int CurrentMileage { get; set; }

    /// <summary>
    /// Vehicle status
    /// </summary>
    public VehicleStatusEnums Status { get; set; }

    /// <summary>
    /// Purchase date
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// Purchase price
    /// </summary>
    public decimal? PurchasePrice { get; set; }

    /// <summary>
    /// Insurance policy number
    /// </summary>
    public string? InsurancePolicyNumber { get; set; }

    /// <summary>
    /// Insurance expiry date
    /// </summary>
    public DateTime? InsuranceExpiryDate { get; set; }

    /// <summary>
    /// Next maintenance due date
    /// </summary>
    public DateTime? NextMaintenanceDue { get; set; }

    /// <summary>
    /// Next maintenance due mileage
    /// </summary>
    public int? NextMaintenanceMileage { get; set; }

    /// <summary>
    /// Currently assigned driver
    /// </summary>
    public string? AssignedDriverName { get; set; }

    /// <summary>
    /// Vehicle notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Vehicle full name (Brand Model Year)
    /// </summary>
    public string FullName => $"{Brand} {Model} ({Year})";

    /// <summary>
    /// Alias for CurrentMileage (for compatibility)
    /// </summary>
    public int Mileage => CurrentMileage;

    /// <summary>
    /// Alias for Vin (for compatibility)
    /// </summary>
    public string? VIN => Vin;
}