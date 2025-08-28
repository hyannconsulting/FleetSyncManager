using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.DTOs;

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
    public MaintenanceTypeEnums Type { get; set; }

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
