using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Application.Interfaces
{
    /// <summary>
    /// Interface pour la gestion des conducteurs (CRUD, pagination, etc.)
    /// </summary>
    public interface IDriverService
    {
        /// <summary>
        /// Récupère tous les conducteurs.
        /// </summary>
        Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Récupère un conducteur par son identifiant.
        /// </summary>
        Task<DriverDto?> GetDriverByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Crée un nouveau conducteur.
        /// </summary>
        Task<DriverDto> CreateDriverAsync(CreateDriverDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Met à jour un conducteur existant.
        /// </summary>
        Task<DriverDto> UpdateDriverAsync(int id, UpdateDriverDto dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Supprime un conducteur par son identifiant.
        /// </summary>
        Task DeleteDriverAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves the count of items.
        /// </summary>
        /// <remarks>The operation may be canceled by passing a cancellation token. If the token is
        /// canceled before the operation completes,  the returned task will be in a canceled state.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the count of items as an
        /// integer.</returns>
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    }
}
