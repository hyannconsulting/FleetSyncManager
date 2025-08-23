using Laroche.FleetManager.Domain.Common;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Vehicle domain entity
/// </summary>
public class Vehicle : BaseEntity
{
    /// <summary>
    /// Vehicle license plate number
    /// </summary>
    public string LicensePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Vehicle Identification Number (VIN)
    /// </summary>
    public string? Vin { get; set; }
    
    /// <summary>
    /// Vehicle brand/manufacturer
    /// </summary>
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>
    /// Vehicle model
    /// </summary>
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// Vehicle manufacturing year
    /// </summary>
    public int Year { get; set; }
    
    /// <summary>
    /// Vehicle fuel type
    /// </summary>
    public FuelType FuelType { get; set; }
    
    /// <summary>
    /// Current mileage/kilometers
    /// </summary>
    public int CurrentMileage { get; set; }
    
    /// <summary>
    /// Vehicle current status
    /// </summary>
    public VehicleStatus Status { get; set; } = VehicleStatus.Active;
    
    /// <summary>
    /// Vehicle purchase date
    /// </summary>
    public DateTime? PurchaseDate { get; set; }
    
    /// <summary>
    /// Vehicle purchase price
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
    /// Vehicle assignments to drivers
    /// </summary>
    public virtual ICollection<VehicleAssignment> VehicleAssignments { get; set; } = [];
    
    /// <summary>
    /// Vehicle maintenance records
    /// </summary>
    public virtual ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = [];
    
    /// <summary>
    /// Vehicle incidents
    /// </summary>
    public virtual ICollection<Incident> Incidents { get; set; } = [];
    
    /// <summary>
    /// Vehicle GPS tracking records
    /// </summary>
    public virtual ICollection<GpsTrackingRecord> GpsTrackingRecords { get; set; } = [];
}
