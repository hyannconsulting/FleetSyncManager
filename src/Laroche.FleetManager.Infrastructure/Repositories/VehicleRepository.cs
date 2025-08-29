using Microsoft.EntityFrameworkCore;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;
using Laroche.FleetManager.Infrastructure.Data;

// Alias pour simplifier l'usage
using VehicleStatus = Laroche.FleetManager.Domain.Enums.VehicleStatusEnums;

namespace Laroche.FleetManager.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour les véhicules
/// </summary>
public class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;

    public VehicleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        string? brand = null,
        VehicleStatus? status = null,
        FuelType? fuelType = null,
        string sortBy = "LicensePlate",
        string sortDirection = "asc",
        CancellationToken cancellationToken = default)
    {
        // Construction de la requête de base
        var query = _context.Vehicles
            .Where(v => !v.IsDeleted)
            .AsQueryable();

        // Application des filtres
        if (!string.IsNullOrEmpty(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(v =>
                v.LicensePlate.ToLower().Contains(search) ||
                v.Brand.ToLower().Contains(search) ||
                v.Model.ToLower().Contains(search) ||
                (v.Vin != null && v.Vin.ToLower().Contains(search)));
        }

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(v => v.Brand.ToLower() == brand.ToLower());
        }

        if (status.HasValue)
        {
            query = query.Where(v => v.Status == status.Value);
        }

        if (fuelType.HasValue)
        {
            query = query.Where(v => v.FuelType == fuelType.Value);
        }

        // Application du tri
        query = sortBy.ToLower() switch
        {
            "brand" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.Brand) 
                : query.OrderBy(v => v.Brand),
            "model" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.Model) 
                : query.OrderBy(v => v.Model),
            "year" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.Year) 
                : query.OrderBy(v => v.Year),
            "status" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.Status) 
                : query.OrderBy(v => v.Status),
            "currentmileage" => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.CurrentMileage) 
                : query.OrderBy(v => v.CurrentMileage),
            _ => sortDirection.ToLower() == "desc" 
                ? query.OrderByDescending(v => v.LicensePlate) 
                : query.OrderBy(v => v.LicensePlate)
        };

        // Comptage total
        var totalCount = await query.CountAsync(cancellationToken);

        // Pagination
        var vehicles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(v => v.VehicleAssignments.Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow))
                .ThenInclude(va => va.Driver)
            .ToListAsync(cancellationToken);

        return (vehicles, totalCount);
    }

    public async Task<Vehicle?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Vehicles
            .Where(v => v.Id == id && !v.IsDeleted)
            .Include(v => v.VehicleAssignments.Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow))
                .ThenInclude(va => va.Driver)
            .Include(v => v.MaintenanceRecords)
            .Include(v => v.Incidents)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        vehicle.CreatedAt = DateTime.UtcNow;
        
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        
        return vehicle;
    }

    public async Task<Vehicle> UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        vehicle.UpdatedAt = DateTime.UtcNow;
        
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        
        return vehicle;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted, cancellationToken);
            
        if (vehicle != null)
        {
            vehicle.IsDeleted = true;
            vehicle.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> LicensePlateExistsAsync(string licensePlate, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Vehicles
            .Where(v => v.LicensePlate.ToLower() == licensePlate.ToLower() && !v.IsDeleted);
            
        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(cancellationToken);
    }
}
