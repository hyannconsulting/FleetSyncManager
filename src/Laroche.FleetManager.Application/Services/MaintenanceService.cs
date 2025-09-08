
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;

        public MaintenanceService(IMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await _maintenanceRepository.GetCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<MaintenanceDto>> GetAllAsync()
        {
            var entities = await _maintenanceRepository.GetAllAsync();
            return entities.Select(MapToDto).ToList();
        }

        public async Task<MaintenanceDto?> GetMaintenanceByIdAsync(int id)
        {
            var entity = await _maintenanceRepository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<MaintenanceDto> CreateMaintenanceAsync(CreateMaintenanceDto dto)
        {
            var entity = new MaintenanceRecord
            {
                VehicleId = dto.VehicleId,
                Description = dto.Description,
                ScheduledDate = dto.Date,
                // Les autres champs sont optionnels ou non présents dans le DTO de création
            };
            var created = await _maintenanceRepository.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<MaintenanceDto> UpdateMaintenanceAsync(int id, UpdateMaintenanceDto dto)
        {
            var entity = new MaintenanceRecord
            {
                Id = id,
                VehicleId = dto.VehicleId,
                Description = dto.Description,
                ScheduledDate = dto.Date,
                // Les autres champs sont optionnels ou non présents dans le DTO de mise à jour
            };
            var updated = await _maintenanceRepository.UpdateAsync(id, entity);
            return MapToDto(updated);
        }

        public async Task DeleteMaintenanceAsync(int id)
        {
            await _maintenanceRepository.DeleteAsync(id);
        }

        private static MaintenanceDto MapToDto(MaintenanceRecord m)
        {
            return new MaintenanceDto
            {
                Id = m.Id,
                VehicleId = m.VehicleId,
                Description = m.Description,
                ScheduledDate = m.ScheduledDate,
                CompletedDate = m.CompletedDate,
                ActualCost = m.Cost,
                ServiceProvider = m.ServiceProvider,
                MileageAtMaintenance = m.MileageAtMaintenance,
                Notes = m.Notes ?? string.Empty,
            };
        }
    }
}
