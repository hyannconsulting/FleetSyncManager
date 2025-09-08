using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Infrastructure.Data;

namespace Laroche.FleetManager.Infrastructure.Repositories
{
    public class MaintenanceRepository : Laroche.FleetManager.Application.Interfaces.IMaintenanceRepository
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.MaintenanceRecords.CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.MaintenanceRecords.ToListAsync(cancellationToken);
        }

        public async Task<MaintenanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.MaintenanceRecords.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<MaintenanceRecord> CreateAsync(MaintenanceRecord entity, CancellationToken cancellationToken = default)
        {
            _context.MaintenanceRecords.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<MaintenanceRecord> UpdateAsync(int id, MaintenanceRecord entity, CancellationToken cancellationToken = default)
        {
            var existing = await _context.MaintenanceRecords.FindAsync(new object[] { id }, cancellationToken);
            if (existing == null) throw new KeyNotFoundException();
            // Copier les propriétés nécessaires
            existing.VehicleId = entity.VehicleId;
            existing.Description = entity.Description;
            existing.ScheduledDate = entity.ScheduledDate;
            existing.CompletedDate = entity.CompletedDate;
            existing.Cost = entity.Cost;
            existing.ServiceProvider = entity.ServiceProvider;
            existing.MileageAtMaintenance = entity.MileageAtMaintenance;
            existing.Notes = entity.Notes;
            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.MaintenanceRecords.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null)
            {
                _context.MaintenanceRecords.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
