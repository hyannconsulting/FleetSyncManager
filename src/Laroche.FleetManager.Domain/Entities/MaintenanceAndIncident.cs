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
    public MaintenanceTypeEnums Type { get; set; }

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
