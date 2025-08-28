using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Commands.Vehicles;

/// <summary>
/// Command to update an existing vehicle
/// </summary>
public class UpdateVehicleCommand : IRequest<Result<VehicleDto>>
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
    public string FuelType { get; set; } = string.Empty;
    
    /// <summary>
    /// Current mileage
    /// </summary>
    public int CurrentMileage { get; set; }
    
    /// <summary>
    /// Vehicle status
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Purchase date
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// Vehicle notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Alias for CurrentMileage (for compatibility)
    /// </summary>
    public int Mileage
    {
        get => CurrentMileage;
        set => CurrentMileage = value;
    }

    /// <summary>
    /// Alias for Vin (for compatibility)
    /// </summary>
    public string? VIN
    {
        get => Vin;
        set => Vin = value;
    }
}
