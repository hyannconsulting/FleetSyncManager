namespace Laroche.FleetManager.Application.DTOs;

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

    /// <summary>
    /// Alias for ScheduledDate (for compatibility)
    /// </summary>
    public DateTime StartDate => ScheduledDate;

    /// <summary>
    /// Alias for CompletedDate (for compatibility)
    /// </summary>
    public DateTime? EndDate => CompletedDate;

    /// <summary>
    /// Alias for ServiceProvider (for compatibility)
    /// </summary>
    public string? Vendor => ServiceProvider;
}
