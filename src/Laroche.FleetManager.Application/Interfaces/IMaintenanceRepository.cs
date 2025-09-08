using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Interfaces
{
    public interface IMaintenanceRepository
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<MaintenanceRecord>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<MaintenanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<MaintenanceRecord> CreateAsync(MaintenanceRecord entity, CancellationToken cancellationToken = default);
        Task<MaintenanceRecord> UpdateAsync(int id, MaintenanceRecord entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
