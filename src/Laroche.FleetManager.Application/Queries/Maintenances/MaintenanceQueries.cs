using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Queries.Maintenances;

/// <summary>
/// Query to get all maintenance records with pagination
/// </summary>
public class GetAllMaintenancesQuery : IRequest<Result<PagedResult<MaintenanceDto>>>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by maintenance status
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Filter by maintenance type
    /// </summary>
    public string? MaintenanceType { get; set; }
    
    /// <summary>
    /// Filter by priority
    /// </summary>
    public string? Priority { get; set; }
    
    /// <summary>
    /// Filter by vehicle ID
    /// </summary>
    public int? VehicleId { get; set; }
}

/// <summary>
/// Query to get a maintenance record by ID
/// </summary>
public class GetMaintenanceByIdQuery : IRequest<Result<MaintenanceDto>>
{
    /// <summary>
    /// Maintenance ID
    /// </summary>
    public int Id { get; set; }
    
    public GetMaintenanceByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get maintenance records for a specific vehicle
/// </summary>
public class GetMaintenancesByVehicleQuery : IRequest<Result<IReadOnlyList<MaintenanceDto>>>
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Include completed maintenances
    /// </summary>
    public bool IncludeCompleted { get; set; } = true;
    
    /// <summary>
    /// Maximum number of records to return
    /// </summary>
    public int? MaxRecords { get; set; }
    
    public GetMaintenancesByVehicleQuery(int vehicleId)
    {
        VehicleId = vehicleId;
    }
}

/// <summary>
/// Query to get upcoming maintenance records
/// </summary>
public class GetUpcomingMaintenancesQuery : IRequest<Result<IReadOnlyList<MaintenanceDto>>>
{
    /// <summary>
    /// Number of days ahead to look for upcoming maintenances
    /// </summary>
    public int DaysAhead { get; set; } = 30;
    
    /// <summary>
    /// Include overdue maintenances
    /// </summary>
    public bool IncludeOverdue { get; set; } = true;
    
    /// <summary>
    /// Filter by priority
    /// </summary>
    public string? Priority { get; set; }
    
    /// <summary>
    /// Filter by maintenance type
    /// </summary>
    public string? MaintenanceType { get; set; }
}

/// <summary>
/// Query to get overdue maintenance records
/// </summary>
public class GetOverdueMaintenancesQuery : IRequest<Result<IReadOnlyList<MaintenanceDto>>>
{
    /// <summary>
    /// Filter by priority
    /// </summary>
    public string? Priority { get; set; }
    
    /// <summary>
    /// Filter by vehicle ID
    /// </summary>
    public int? VehicleId { get; set; }
}

/// <summary>
/// Query to get maintenance statistics
/// </summary>
public class GetMaintenanceStatisticsQuery : IRequest<Result<MaintenanceStatisticsDto>>
{
    /// <summary>
    /// Date range start
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// Date range end
    /// </summary>
    public DateTime? ToDate { get; set; }
    
    /// <summary>
    /// Filter by vehicle ID
    /// </summary>
    public int? VehicleId { get; set; }
}
