using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Queries.Vehicles;

/// <summary>
/// Query to get all vehicles with pagination and filtering
/// </summary>
public class GetVehiclesQuery : IRequest<Result<PagedResult<VehicleDto>>>
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// Search term for filtering
    /// </summary>
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by brand
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Filter by fuel type
    /// </summary>
    public string? FuelType { get; set; }
    
    /// <summary>
    /// Sort field
    /// </summary>
    public string SortBy { get; set; } = "LicensePlate";
    
    /// <summary>
    /// Sort direction (asc/desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// Query to get a vehicle by ID
/// </summary>
public class GetVehicleByIdQuery : IRequest<Result<VehicleDto?>>
{
    /// <summary>
    /// Vehicle ID
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Initializes the query
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    public GetVehicleByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get vehicles assigned to a specific driver
/// </summary>
public class GetVehiclesByDriverQuery : IRequest<Result<IReadOnlyList<VehicleDto>>>
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int DriverId { get; set; }
    
    /// <summary>
    /// Include inactive assignments
    /// </summary>
    public bool IncludeInactive { get; set; } = false;
    
    /// <summary>
    /// Initializes the query
    /// </summary>
    /// <param name="driverId">Driver ID</param>
    public GetVehiclesByDriverQuery(int driverId)
    {
        DriverId = driverId;
    }
}

/// <summary>
/// Query to get vehicles that need maintenance
/// </summary>
public class GetVehiclesNeedingMaintenanceQuery : IRequest<Result<IReadOnlyList<VehicleDto>>>
{
    /// <summary>
    /// Number of days ahead to look for maintenance
    /// </summary>
    public int DaysAhead { get; set; } = 30;
    
    /// <summary>
    /// Include overdue maintenance
    /// </summary>
    public bool IncludeOverdue { get; set; } = true;
}
