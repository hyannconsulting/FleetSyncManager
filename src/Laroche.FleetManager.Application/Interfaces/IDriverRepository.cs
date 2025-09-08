using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Interfaces;

/// <summary>
/// Interface for Driver repository operations
/// </summary>
public interface IDriverRepository
{
    /// <summary>
    /// Get all drivers with optional filtering and pagination
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="licenseType">Optional license type filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged result of drivers</returns>
    Task<PagedResult<Driver>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? status = null,
        string? licenseType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a driver by ID
    /// </summary>
    /// <param name="id">Driver ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Driver entity or null if not found</returns>
    Task<Driver?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get drivers with expiring licenses
    /// </summary>
    /// <param name="daysAhead">Number of days ahead to check for expiring licenses</param>
    /// <param name="includeExpired">Whether to include already expired licenses</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of drivers with expiring licenses</returns>
    Task<IReadOnlyList<Driver>> GetDriversWithExpiringLicensesAsync(
        int daysAhead = 30,
        bool includeExpired = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available drivers for assignment
    /// </summary>
    /// <param name="requiredLicenseType">Required license type</param>
    /// <param name="onlyValidLicenses">Only include drivers with valid licenses</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of available drivers</returns>
    Task<IReadOnlyList<Driver>> GetAvailableDriversAsync(
        string? requiredLicenseType = null,
        bool onlyValidLicenses = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new driver
    /// </summary>
    /// <param name="driver">Driver to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created driver</returns>
    Task<Driver> CreateAsync(Driver driver, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing driver
    /// </summary>
    /// <param name="driver">Driver to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated driver</returns>
    Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a driver by ID
    /// </summary>
    /// <param name="id">Driver ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email address is unique (excluding specific driver ID)
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="excludeDriverId">Driver ID to exclude from check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email is unique</returns>
    Task<bool> IsEmailUniqueAsync(string email, int? excludeDriverId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if license number is unique (excluding specific driver ID)
    /// </summary>
    /// <param name="licenseNumber">License number to check</param>
    /// <param name="excludeDriverId">Driver ID to exclude from check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if license number is unique</returns>
    Task<bool> IsLicenseNumberUniqueAsync(string licenseNumber, int? excludeDriverId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the count of items.
    /// </summary>
    /// <remarks>This method is designed to be non-blocking and supports cancellation through the provided
    /// <paramref name="cancellationToken"/>.</remarks>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the count of items as an integer.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
