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
    public VehicleStatus Status { get; set; }
    
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
    /// Vehicle full name (Brand Model Year)
    /// </summary>
    public string FullName => $"{Brand} {Model} ({Year})";
}

/// <summary>
/// Driver Data Transfer Object
/// </summary>
public class DriverDto
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Driver first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver email
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Driver license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// License type
    /// </summary>
    public LicenseType LicenseType { get; set; }
    
    /// <summary>
    /// License expiry date
    /// </summary>
    public DateTime LicenseExpiryDate { get; set; }
    
    /// <summary>
    /// Hire date
    /// </summary>
    public DateTime HireDate { get; set; }
    
    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// Driver status
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Currently assigned vehicles
    /// </summary>
    public List<string> AssignedVehicles { get; set; } = [];
    
    /// <summary>
    /// Driver full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}

/// <summary>
/// Maintenance Record DTO
/// </summary>
public class MaintenanceRecordDto
{
    /// <summary>
    /// Record ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Vehicle license plate
    /// </summary>
    public string VehicleLicensePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Maintenance type
    /// </summary>
    public MaintenanceType Type { get; set; }
    
    /// <summary>
    /// Description
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
    /// Cost
    /// </summary>
    public decimal Cost { get; set; }
    
    /// <summary>
    /// Service provider
    /// </summary>
    public string? ServiceProvider { get; set; }
    
    /// <summary>
    /// Mileage at maintenance
    /// </summary>
    public int MileageAtMaintenance { get; set; }
    
    /// <summary>
    /// Is completed
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Is overdue
    /// </summary>
    public bool IsOverdue { get; set; }
}

/// <summary>
/// Incident DTO
/// </summary>
public class IncidentDto
{
    /// <summary>
    /// Incident ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Vehicle license plate
    /// </summary>
    public string VehicleLicensePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Driver ID
    /// </summary>
    public int? DriverId { get; set; }
    
    /// <summary>
    /// Driver name
    /// </summary>
    public string? DriverName { get; set; }
    
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
    /// Incident date
    /// </summary>
    public DateTime IncidentDate { get; set; }
    
    /// <summary>
    /// Location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Estimated cost
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Actual cost
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Is resolved
    /// </summary>
    public bool IsResolved { get; set; }
    
    /// <summary>
    /// Resolution date
    /// </summary>
    public DateTime? ResolutionDate { get; set; }
}

/// <summary>
/// Maintenance DTO (compatible with commands)
/// </summary>
public class MaintenanceDto
{
    /// <summary>
    /// Maintenance ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Vehicle license plate
    /// </summary>
    public string VehicleLicensePlate { get; set; } = string.Empty;
    
    /// <summary>
    /// Maintenance type
    /// </summary>
    public string MaintenanceType { get; set; } = string.Empty;
    
    /// <summary>
    /// Description
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
    /// Estimated cost
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Actual cost
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Service provider
    /// </summary>
    public string? ServiceProvider { get; set; }
    
    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; set; } = "Medium";
    
    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = "Planned";
    
    /// <summary>
    /// Vehicle mileage at maintenance
    /// </summary>
    public int? MileageAtMaintenance { get; set; }
    
    /// <summary>
    /// Whether this is preventive maintenance
    /// </summary>
    public bool IsPreventive { get; set; }
    
    /// <summary>
    /// Notes
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Is completed
    /// </summary>
    public bool IsCompleted => CompletedDate.HasValue;
    
    /// <summary>
    /// Is overdue
    /// </summary>
    public bool IsOverdue => !IsCompleted && ScheduledDate < DateTime.Today;
}

/// <summary>
/// Maintenance statistics DTO
/// </summary>
public class MaintenanceStatisticsDto
{
    /// <summary>
    /// Total maintenance records
    /// </summary>
    public int TotalMaintenances { get; set; }
    
    /// <summary>
    /// Completed maintenances
    /// </summary>
    public int CompletedMaintenances { get; set; }
    
    /// <summary>
    /// Pending maintenances
    /// </summary>
    public int PendingMaintenances { get; set; }
    
    /// <summary>
    /// Overdue maintenances
    /// </summary>
    public int OverdueMaintenances { get; set; }
    
    /// <summary>
    /// Total cost
    /// </summary>
    public decimal TotalCost { get; set; }
    
    /// <summary>
    /// Average cost per maintenance
    /// </summary>
    public decimal AverageCost { get; set; }
    
    /// <summary>
    /// Most common maintenance type
    /// </summary>
    public string? MostCommonType { get; set; }
    
    /// <summary>
    /// Maintenance by type
    /// </summary>
    public Dictionary<string, int> MaintenanceByType { get; set; } = new();
}

/// <summary>
/// Incident statistics DTO
/// </summary>
public class IncidentStatisticsDto
{
    /// <summary>
    /// Total incidents
    /// </summary>
    public int TotalIncidents { get; set; }
    
    /// <summary>
    /// Open incidents
    /// </summary>
    public int OpenIncidents { get; set; }
    
    /// <summary>
    /// Resolved incidents
    /// </summary>
    public int ResolvedIncidents { get; set; }
    
    /// <summary>
    /// Total estimated cost
    /// </summary>
    public decimal TotalEstimatedCost { get; set; }
    
    /// <summary>
    /// Total actual cost
    /// </summary>
    public decimal TotalActualCost { get; set; }
    
    /// <summary>
    /// Average resolution time in days
    /// </summary>
    public double AverageResolutionDays { get; set; }
    
    /// <summary>
    /// Most common incident type
    /// </summary>
    public string? MostCommonType { get; set; }
    
    /// <summary>
    /// Incidents by severity
    /// </summary>
    public Dictionary<string, int> IncidentsBySeverity { get; set; } = new();
    
    /// <summary>
    /// Incidents by type
    /// </summary>
    public Dictionary<string, int> IncidentsByType { get; set; } = new();
}
