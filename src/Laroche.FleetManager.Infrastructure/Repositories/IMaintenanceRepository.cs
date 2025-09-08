using System.Threading;
using System.Threading.Tasks;

namespace Laroche.FleetManager.Infrastructure.Repositories
{
    public interface IMaintenanceRepository
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        // ... autres méthodes CRUD à ajouter si besoin ...
    }
}
