using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Incidents;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Interfaces
{
    public interface IIncidentRepository
    {
        /// <summary>
        /// Retrieves an incident by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to retrieve an incident from the data
        /// source. If no incident with the specified <paramref name="id"/> exists, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="id">The unique identifier of the incident to retrieve. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Incident"/> if
        /// found; otherwise, <see langword="null"/>.</returns>
        Task<Incident?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves all incidents.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/>
        /// of <see cref="Incident"/> objects representing all incidents.</returns>
        Task<IEnumerable<Incident>> GetAllAsync(CancellationToken cancellationToken = default);
        //   Task<IEnumerable<Incident>> GetByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken = default);
        //Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status);
        //Task<IEnumerable<Incident>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Creates a new incident and adds it to the system.
        /// </summary>
        /// <param name="incident">The incident to create. Must not be <see langword="null"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created incident with its
        /// assigned identifier and any additional system-generated properties.</returns>
        Task<Incident> CreateAsync(Incident incident, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the specified incident and returns the updated instance.
        /// </summary>
        /// <param name="incident">The incident to update. Must not be <see langword="null"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see
        /// cref="Incident"/>.</returns>
        Task<Incident> UpdateAsync(Incident incident, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the entity with the specified identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to check for existence.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the entity
        /// exists; otherwise, <see langword="false"/>.</returns>
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for incidents that match the specified search term.
        /// </summary>
        /// <remarks>The search operation is case-insensitive and may involve partial matches depending on
        /// the implementation.</remarks>
        /// <param name="searchTerm">The term to search for. This value cannot be null or empty.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of  <see
        /// cref="Incident"/> objects that match the search term. If no matches are found, the collection will be empty.</returns>
        Task<IEnumerable<Incident>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves the total count of items.
        /// </summary>
        /// <remarks>The method returns the count of items as an integer. If the operation is canceled via
        /// the provided <paramref name="cancellationToken"/>, the returned task will be in a canceled state.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the total count of items.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        // Task<IEnumerable<Incident>> GetPaginatedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);


        /// <summary>
        /// Marks the incident with the specified identifier as resolved.
        /// </summary>
        /// <remarks>This method updates the status of the incident to indicate that it has been resolved.
        /// Ensure the specified <paramref name="id"/> corresponds to a valid incident.</remarks>
        /// <param name="id">The unique identifier of the incident to mark as resolved.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns></returns>
        Task MarkAsResolvedAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a paginated list of incidents based on the specified query parameters.
        /// </summary>
        /// <remarks>The method supports pagination and filtering based on the properties of the <paramref
        /// name="query"/> parameter. Ensure that the query parameters are valid and within the expected range to avoid
        /// unexpected results.</remarks>
        /// <param name="query">The query parameters used to filter and paginate the incidents. Must not be <see langword="null"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. Passing a canceled token will immediately terminate the
        /// operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{T}"/>
        /// of <see cref="IncidentDto"/> objects that match the query parameters. If no incidents are found, the result
        /// will contain an empty collection.</returns>
        Task<PagedResult<IncidentDto>> GetPagedAsync(GetAllIncidentsQuery query, CancellationToken cancellationToken);
    }
}
