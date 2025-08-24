using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Commands.Maintenances;

/// <summary>
/// Command to create a new maintenance record
/// </summary>
public class CreateMaintenanceCommand : IRequest<Result<MaintenanceDto>>
{
    /// <summary>
    /// Vehicle ID for this maintenance
    /// </summary>
    public int VehicleId { get; set; }
    
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
    /// Estimated cost
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    
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
    /// Whether this is preventive or corrective maintenance
    /// </summary>
    public bool IsPreventive { get; set; } = true;
    
    /// <summary>
    /// Notes or additional information
    /// </summary>
    public string? Notes { get; set; }
}

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
}

/// <summary>
/// Command to delete a maintenance record
/// </summary>
public class DeleteMaintenanceCommand : IRequest<Result>
{
    /// <summary>
    /// Maintenance ID to delete
    /// </summary>
    public int Id { get; set; }
    
    public DeleteMaintenanceCommand(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Command to complete a maintenance
/// </summary>
public class CompleteMaintenanceCommand : IRequest<Result<MaintenanceDto>>
{
    /// <summary>
    /// Maintenance ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Completion date
    /// </summary>
    public DateTime CompletedDate { get; set; }
    
    /// <summary>
    /// Actual cost
    /// </summary>
    public decimal? ActualCost { get; set; }
    
    /// <summary>
    /// Vehicle mileage when completed
    /// </summary>
    public int? CompletionMileage { get; set; }
    
    /// <summary>
    /// Completion notes
    /// </summary>
    public string? CompletionNotes { get; set; }
    
    public CompleteMaintenanceCommand(int id)
    {
        Id = id;
        CompletedDate = DateTime.Now;
    }
}
