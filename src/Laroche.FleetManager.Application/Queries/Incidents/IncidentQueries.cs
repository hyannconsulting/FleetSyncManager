using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Queries.Incidents;

/// <summary>
/// Query to get all incidents with pagination
/// </summary>
public class GetAllIncidentsQuery : IRequest<Result<PagedResult<IncidentDto>>>
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
    /// Filter by incident status
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Filter by incident type
    /// </summary>
    public string? IncidentType { get; set; }
    
    /// <summary>
    /// Filter by severity
    /// </summary>
    public string? Severity { get; set; }
    
    /// <summary>
    /// Filter by vehicle ID
    /// </summary>
    public int? VehicleId { get; set; }
    
    /// <summary>
    /// Filter by driver ID
    /// </summary>
    public int? DriverId { get; set; }
    
    /// <summary>
    /// Filter from date
    /// </summary>
    public DateTime? FromDate { get; set; }
    
    /// <summary>
    /// Filter to date
    /// </summary>
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Query to get an incident by ID
/// </summary>
public class GetIncidentByIdQuery : IRequest<Result<IncidentDto>>
{
    /// <summary>
    /// Incident ID
    /// </summary>
    public int Id { get; set; }
    
    public GetIncidentByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get incidents for a specific vehicle
/// </summary>
public class GetIncidentsByVehicleQuery : IRequest<Result<IReadOnlyList<IncidentDto>>>
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int VehicleId { get; set; }
    
    /// <summary>
    /// Include resolved incidents
    /// </summary>
    public bool IncludeResolved { get; set; } = true;
    
    /// <summary>
    /// Maximum number of records to return
    /// </summary>
    public int? MaxRecords { get; set; }
    
    public GetIncidentsByVehicleQuery(int vehicleId)
    {
        VehicleId = vehicleId;
    }
}

/// <summary>
/// Query to get incidents for a specific driver
/// </summary>
public class GetIncidentsByDriverQuery : IRequest<Result<IReadOnlyList<IncidentDto>>>
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int DriverId { get; set; }
    
    /// <summary>
    /// Include resolved incidents
    /// </summary>
    public bool IncludeResolved { get; set; } = true;
    
    /// <summary>
    /// Maximum number of records to return
    /// </summary>
    public int? MaxRecords { get; set; }
    
    public GetIncidentsByDriverQuery(int driverId)
    {
        DriverId = driverId;
    }
}

/// <summary>
/// Query to get open/unresolved incidents
/// </summary>
public class GetOpenIncidentsQuery : IRequest<Result<IReadOnlyList<IncidentDto>>>
{
    /// <summary>
    /// Filter by severity
    /// </summary>
    public string? Severity { get; set; }
    
    /// <summary>
    /// Filter by incident type
    /// </summary>
    public string? IncidentType { get; set; }
    
    /// <summary>
    /// Filter by vehicle ID
    /// </summary>
    public int? VehicleId { get; set; }
    
    /// <summary>
    /// Order by incident date (default: newest first)
    /// </summary>
    public bool OrderByDateDesc { get; set; } = true;
}

/// <summary>
/// Query to get recent incidents
/// </summary>
public class GetRecentIncidentsQuery : IRequest<Result<IReadOnlyList<IncidentDto>>>
{
    /// <summary>
    /// Number of days to look back
    /// </summary>
    public int DaysBack { get; set; } = 30;
    
    /// <summary>
    /// Maximum number of records to return
    /// </summary>
    public int MaxRecords { get; set; } = 50;
    
    /// <summary>
    /// Filter by severity
    /// </summary>
    public string? Severity { get; set; }
}

/// <summary>
/// Query to get incident statistics
/// </summary>
public class GetIncidentStatisticsQuery : IRequest<Result<IncidentStatisticsDto>>
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
    
    /// <summary>
    /// Filter by driver ID
    /// </summary>
    public int? DriverId { get; set; }
}
