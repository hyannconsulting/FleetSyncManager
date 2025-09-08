
using Laroche.FleetManager.Application.Commands.Vehicles;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Vehicles;

namespace Laroche.FleetManager.Web.Services.Interfaces;

/// <summary>
/// Interface pour le service client API des véhicules
/// </summary>
public interface IVehicleApiService : IApiClientService<VehicleDto, CreateVehicleCommand, UpdateVehicleCommand>
{
    /// <summary>
    /// Retrieves a collection of vehicles associated with the specified driver.
    /// </summary>
    /// <param name="driverId">The unique identifier of the driver whose vehicles are to be retrieved.</param>
    /// <param name="includeInactive">A value indicating whether to include inactive vehicles in the result.  <see langword="true"/> to include
    /// inactive vehicles; otherwise, <see langword="false"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an  <see cref="IEnumerable{T}"/> of
    /// <see cref="VehicleDto"/> objects representing the vehicles  associated with the driver. Returns <see
    /// langword="null"/> if no vehicles are found.</returns>
    Task<IEnumerable<VehicleDto>?> GetVehiclesByDriverAsync(int driverId, bool includeInactive = false);

    /// <summary>
    /// Asynchronously retrieves a collection of vehicles that are due for maintenance within the specified number of
    /// days.
    /// </summary>
    /// <remarks>This method is typically used to identify vehicles requiring maintenance within a specified
    /// time frame.  Ensure that <paramref name="daysAhead"/> is non-negative to avoid an <see
    /// cref="ArgumentOutOfRangeException"/>.</remarks>
    /// <param name="daysAhead">The number of days ahead to check for upcoming maintenance. Must be a non-negative value. Defaults to 30.</param>
    /// <param name="includeOverdue">A value indicating whether to include vehicles that are already overdue for maintenance. Defaults to <see
    /// langword="true"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="VehicleDto"/> objects representing the vehicles needing maintenance, or <see langword="null"/> if no
    /// vehicles are found.</returns>
    Task<IEnumerable<VehicleDto>?> GetVehiclesNeedingMaintenanceAsync(int daysAhead = 30, bool includeOverdue = true);

    /// <summary>
    /// Retrieves a paginated list of vehicles based on the specified query parameters.
    /// </summary>
    /// <remarks>The method supports pagination and filtering to efficiently retrieve large datasets.  Ensure that the
    /// query parameters are valid and within acceptable ranges to avoid unexpected results.</remarks>
    /// <param name="query">The query parameters used to filter, sort, and paginate the vehicle data.  This includes criteria such as page
    /// number, page size, and optional filters.</param>
    /// <returns>A <see cref="PagedResult{T}"/> containing a collection of <see cref="VehicleDto"/> objects  that match the query
    /// parameters, or <see langword="null"/> if no results are found.</returns>
    Task<PagedResult<VehicleDto>?> GetPagedAsync(GetVehiclesQuery query);

    /// <summary>
    /// Asynchronously retrieves the total count of vehicles that match the specified query criteria.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of vehicles 
    /// matching the query, or <see langword="null"/> if the count cannot be determined.</returns>
    Task<int?> GetTotalCountAsync();
}
