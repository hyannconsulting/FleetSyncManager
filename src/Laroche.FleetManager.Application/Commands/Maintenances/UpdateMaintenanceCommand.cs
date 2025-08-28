using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Commands.Maintenances;

/// <summary>
/// Command to update an existing maintenance record
/// </summary>
public class UpdateMaintenanceCommand : IRequest<Result<MaintenanceDto>>
{
    /// <summary>
    /// Maintenance ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Type of maintenance
    /// </summary>
    public string MaintenanceType { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the maintenance work
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Scheduled date for maintenance
    /// </summary>
    public DateTime ScheduledDate { get; set; }
    
    /// <summary>
    /// Actual completion date (if completed)
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    
    /// <summary>
    /// Estimated cost
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
    /// <summary>
    /// Actual cost (if completed)
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Service provider/garage name
    /// </summary>
    public string? ServiceProvider { get; set; }
    
    /// <summary>
    /// Priority level (Low, Medium, High, Critical)
    /// </summary>
    public string Priority { get; set; } = "Medium";
    
    /// <summary>
    /// Vehicle mileage at time of maintenance
    /// </summary>
    public int? MileageAtMaintenance { get; set; }
    
    /// <summary>
    /// Status of the maintenance
    /// </summary>
    public string Status { get; set; } = "Planned";
    
    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Vehicle ID for this maintenance
    /// </summary>
    public int VehicleId { get; set; }

    /// <summary>
    /// Alias for ScheduledDate (for compatibility)
    /// </summary>
    public DateTime StartDate
    {
        get => ScheduledDate;
        set => ScheduledDate = value;
    }

    /// <summary>
    /// Alias for CompletedDate (for compatibility)
    /// </summary>
    public DateTime? EndDate
    {
        get => CompletedDate;
        set => CompletedDate = value;
    }

    /// <summary>
    /// Alias for ServiceProvider (for compatibility)
    /// </summary>
    public string? Vendor
    {
        get => ServiceProvider;
        set => ServiceProvider = value;
    }
}
