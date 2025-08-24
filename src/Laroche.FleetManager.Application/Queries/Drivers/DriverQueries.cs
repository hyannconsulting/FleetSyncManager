using MediatR;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Queries.Drivers;

/// <summary>
/// Query to get all drivers with pagination
/// </summary>
public class GetAllDriversQuery : IRequest<Result<PagedResult<DriverDto>>>
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
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Filter by license type
    /// </summary>
    public string? LicenseType { get; set; }
}

/// <summary>
/// Query to get a driver by ID
/// </summary>
public class GetDriverByIdQuery : IRequest<Result<DriverDto>>
{
    /// <summary>
    /// Driver ID
    /// </summary>
    public int Id { get; set; }
    
    public GetDriverByIdQuery(int id)
    {
        Id = id;
    }
}

/// <summary>
/// Query to get drivers with expiring licenses
/// </summary>
public class GetDriversWithExpiringLicensesQuery : IRequest<Result<IReadOnlyList<DriverDto>>>
{
    /// <summary>
    /// Number of days ahead to look for expiring licenses
    /// </summary>
    public int DaysAhead { get; set; } = 30;
    
    /// <summary>
    /// Include already expired licenses
    /// </summary>
    public bool IncludeExpired { get; set; } = true;
}

/// <summary>
/// Query to get drivers available for assignment
/// </summary>
public class GetAvailableDriversQuery : IRequest<Result<IReadOnlyList<DriverDto>>>
{
    /// <summary>
    /// Required license type for the assignment
    /// </summary>
    public string? RequiredLicenseType { get; set; }
    
    /// <summary>
    /// Only include drivers with valid (non-expired) licenses
    /// </summary>
    public bool OnlyValidLicenses { get; set; } = true;
}
