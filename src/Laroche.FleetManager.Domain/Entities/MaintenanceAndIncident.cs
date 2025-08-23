using Laroche.FleetManager.Domain.Common;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Maintenance record entity
/// </summary>
public class MaintenanceRecord : BaseEntity
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Maintenance type
    /// </summary>
    public MaintenanceType Type { get; set; }
    
    /// <summary>
    /// Maintenance description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Scheduled date
    /// </summary>
    public DateTime ScheduledDate { get; set; }
    
    /// <summary>
    /// Completed date
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    
    /// <summary>
    /// Maintenance cost
    /// </summary>
    public decimal Cost { get; set; }
    
    /// <summary>
    /// Service provider
    /// </summary>
    public string? ServiceProvider { get; set; }
    
    /// <summary>
    /// Vehicle mileage at maintenance
    /// </summary>
    public int MileageAtMaintenance { get; set; }
    
    /// <summary>
    /// Parts replaced
    /// </summary>
    public string? PartsReplaced { get; set; }
    
    /// <summary>
    /// Maintenance notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Next maintenance due date
    /// </summary>
    public DateTime? NextDueDate { get; set; }
    
    /// <summary>
    /// Next maintenance due mileage
    /// </summary>
    public int? NextDueMileage { get; set; }
    
    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
    
    /// <summary>
    /// Is maintenance completed
    /// </summary>
    public bool IsCompleted => CompletedDate.HasValue;
    
    /// <summary>
    /// Is maintenance overdue
    /// </summary>
    public bool IsOverdue => !IsCompleted && ScheduledDate < DateTime.UtcNow;
}

/// <summary>
/// Incident entity
/// </summary>
public class Incident : BaseEntity
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Driver ID (optional)
    /// </summary>
    public int? DriverId { get; set; }
    
    /// <summary>
    /// Incident title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Incident description
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Incident severity
    /// </summary>
    public IncidentSeverity Severity { get; set; }
    
    /// <summary>
    /// Incident date and time
    /// </summary>
    public DateTime IncidentDate { get; set; }
    
    /// <summary>
    /// Location where incident occurred
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// GPS coordinates (latitude)
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// GPS coordinates (longitude)
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Estimated cost of incident
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Actual cost of incident
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Insurance claim number
    /// </summary>
    public string? InsuranceClaimNumber { get; set; }
    
    /// <summary>
    /// Police report number
    /// </summary>
    public string? PoliceReportNumber { get; set; }
    
    /// <summary>
    /// Third party involved
    /// </summary>
    public bool ThirdPartyInvolved { get; set; }
    
    /// <summary>
    /// Third party details
    /// </summary>
    public string? ThirdPartyDetails { get; set; }
    
    /// <summary>
    /// Incident status (resolved/pending)
    /// </summary>
    public bool IsResolved { get; set; }
    
    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime? ResolutionDate { get; set; }
    
    /// <summary>
    /// Resolution notes
    /// </summary>
    public string? ResolutionNotes { get; set; }
    
    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
    
    /// <summary>
    /// Navigation property to Driver
    /// </summary>
    public virtual Driver? Driver { get; set; }
}

/// <summary>
/// GPS tracking record entity
/// </summary>
public class GpsTrackingRecord : BaseEntity
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// GPS coordinates (latitude)
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// GPS coordinates (longitude)
    /// </summary>
    public double Longitude { get; set; }
    
    /// <summary>
    /// Vehicle speed in km/h
    /// </summary>
    public double Speed { get; set; }
    
    /// <summary>
    /// Heading/direction
    /// </summary>
    public double? Heading { get; set; }
    
    /// <summary>
    /// Altitude in meters
    /// </summary>
    public double? Altitude { get; set; }
    
    /// <summary>
    /// GPS timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }
    
    /// <summary>
    /// Vehicle odometer reading
    /// </summary>
    public int? OdometerReading { get; set; }
    
    /// <summary>
    /// Engine status (on/off)
    /// </summary>
    public bool EngineOn { get; set; }
    
    /// <summary>
    /// Additional sensor data (JSON)
    /// </summary>
    public string? SensorData { get; set; }
    
    /// <summary>
    /// Navigation property to Vehicle
    /// </summary>
    public virtual Vehicle Vehicle { get; set; } = null!;
}
