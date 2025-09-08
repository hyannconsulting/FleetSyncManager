
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Laroche.FleetManager.Domain.Enums;
using Laroche.FleetManager.Domain.Extensions;

namespace Laroche.FleetManager.Infrastructure.Services;

public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

    public async Task<PagedResult<VehicleDto>> GetPagedAsync(GetVehiclesQuery query)
    {
        return await _vehicleRepository.GetPagedAsync(query);
    }

    public async Task<IEnumerable<VehicleDto>> GetAllAsync()
    {
        var vehicles = await _vehicleRepository.GetAllAsync();
        return vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            LicensePlate = v.LicensePlate,
            Brand = v.Brand,
            Model = v.Model,
            Year = v.Year,
            Status = v.Status,
            FuelType = v.FuelType,
            CurrentMileage = v.CurrentMileage,
            Vin = v.Vin
        });
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;
        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Status = vehicle.Status,
            FuelType = vehicle.FuelType,
            CurrentMileage = vehicle.CurrentMileage,
            Vin = vehicle.Vin
        };
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var entity = new Domain.Entities.Vehicle
        {
            LicensePlate = dto.LicensePlate,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Status = string.IsNullOrEmpty(dto.Status) ? VehicleStatusEnums.Active : dto.Status.ToEnum<VehicleStatusEnums>(),
            FuelType = dto.FuelType.ToEnum<FuelType>(),
            CurrentMileage = dto.CurrentMileage,
            Vin = dto.Vin,

        };
        var created = await _vehicleRepository.AddAsync(entity);
        return new VehicleDto
        {
            Id = created.Id,
            LicensePlate = created.LicensePlate,
            Brand = created.Brand,
            Model = created.Model,
            Year = created.Year,
            Status = created.Status,
            FuelType = created.FuelType,
            CurrentMileage = created.CurrentMileage,
            Vin = created.Vin
        };
    }

    public async Task<VehicleDto> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
    {
        var entity = await _vehicleRepository.GetByIdAsync(id);
        if (entity == null) throw new Exception("Véhicule non trouvé");

        entity.LicensePlate = dto.LicensePlate;
        entity.Brand = dto.Brand;
        entity.Model = dto.Model;
        entity.Year = dto.Year;
        entity.Status = string.IsNullOrEmpty(dto.Status) ? VehicleStatusEnums.Active : dto.Status.ToEnum<VehicleStatusEnums>();
        entity.FuelType = dto.FuelType.ToEnum<FuelType>();
        entity.CurrentMileage = dto.CurrentMileage;
        entity.Vin = dto.Vin;


        await _vehicleRepository.UpdateAsync(entity);

        return new VehicleDto
        {
            Id = entity.Id,
            LicensePlate = entity.LicensePlate,
            Brand = entity.Brand,
            Model = entity.Model,
            Year = entity.Year,
            Status = entity.Status,
            FuelType = entity.FuelType,
            CurrentMileage = entity.CurrentMileage,
            Vin = entity.Vin
        };
    }

    public async Task DeleteVehicleAsync(int id)
    {
        await _vehicleRepository.DeleteAsync(id);
    }

    async Task<int> IVehicleService.GetCountAsync(CancellationToken cancellationToken)
    {
        return await _vehicleRepository.GetCountAsync(cancellationToken);
    }
}
