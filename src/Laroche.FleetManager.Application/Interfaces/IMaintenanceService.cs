using Laroche.FleetManager.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laroche.FleetManager.Application.Interfaces
{
    /// <summary>
    /// Interface pour la gestion des maintenances (CRUD, pagination, etc.)
    /// </summary>
    public interface IMaintenanceService
    {
        /// <summary>
        /// Récupère toutes les maintenances.
        /// </summary>
        Task<IEnumerable<MaintenanceDto>> GetAllAsync();

        /// <summary>
        /// Récupère une maintenance par son identifiant.
        /// </summary>
        Task<MaintenanceDto?> GetMaintenanceByIdAsync(int id);

        /// <summary>
        /// Crée une nouvelle maintenance.
        /// </summary>
        Task<MaintenanceDto> CreateMaintenanceAsync(CreateMaintenanceDto dto);

        /// <summary>
        /// Met à jour une maintenance existante.
        /// </summary>
        Task<MaintenanceDto> UpdateMaintenanceAsync(int id, UpdateMaintenanceDto dto);

        /// <summary>
        /// Supprime une maintenance par son identifiant.
        /// </summary>
        Task DeleteMaintenanceAsync(int id);

        /// <summary>
        /// Récupère le nombre total de maintenances.
        /// </summary>
        Task<int> GetCountAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}
