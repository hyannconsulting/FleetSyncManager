using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;
using Laroche.FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laroche.FleetManager.Infrastructure.Repositories;

/// <summary>
/// Implementation of IDriverRepository using Entity Framework
/// </summary>
public class DriverRepository : IDriverRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DriverRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverRepository"/> class with the specified database context and
    /// logger.
    /// </summary>
    /// <param name="context">The <see cref="ApplicationDbContext"/> used to interact with the database. Cannot be <see langword="null"/>.</param>
    /// <param name="logger">The <see cref="ILogger{DriverRepository}"/> used for logging operations. Cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="logger"/> is <see langword="null"/>.</exception>
    public DriverRepository(ApplicationDbContext context, ILogger<DriverRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedResult<Driver>> GetAllAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? status = null,
        string? licenseType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Drivers
                .Include(d => d.VehicleAssignments.Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow))
                    .ThenInclude(va => va.Vehicle)
                .AsQueryable();

            // Filtres
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearchTerm = searchTerm.ToLower();
                query = query.Where(d =>
                    d.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    d.LastName.ToLower().Contains(lowerSearchTerm) ||
                    d.Email.ToLower().Contains(lowerSearchTerm) ||
                    d.LicenseNumber.ToLower().Contains(lowerSearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var isActive = status.ToLower() == "active";
                query = query.Where(d => d.IsActive == isActive);
            }

            if (!string.IsNullOrWhiteSpace(licenseType) && Enum.TryParse<LicenseTypeEnums>(licenseType, out var licenseTypeEnum))
            {
                query = query.Where(d => d.LicenseType == licenseTypeEnum);
            }

            // Comptage total
            var totalCount = await query.CountAsync(cancellationToken);

            // Pagination et tri
            var drivers = await query
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Récupération de {Count} conducteurs sur {Total}", drivers.Count, totalCount);

            return new PagedResult<Driver>
            {
                Items = drivers,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des conducteurs");
            throw;
        }
    }

    public async Task<Driver?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Drivers
                .Include(d => d.VehicleAssignments.Where(va => va.EndDate == null || va.EndDate > DateTime.UtcNow))
                    .ThenInclude(va => va.Vehicle)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du conducteur {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<Driver>> GetDriversWithExpiringLicensesAsync(
        int daysAhead = 30,
        bool includeExpired = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);
            var today = DateTime.UtcNow;

            var query = _context.Drivers.AsQueryable();

            if (includeExpired)
            {
                query = query.Where(d => d.LicenseExpiryDate <= cutoffDate);
            }
            else
            {
                query = query.Where(d => d.LicenseExpiryDate <= cutoffDate && d.LicenseExpiryDate >= today);
            }

            return await query
                .Where(d => d.IsActive)
                .OrderBy(d => d.LicenseExpiryDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des conducteurs avec permis expirant");
            throw;
        }
    }

    public async Task<IReadOnlyList<Driver>> GetAvailableDriversAsync(
        string? requiredLicenseType = null,
        bool onlyValidLicenses = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Drivers
                .Where(d => d.IsActive);

            // Filtre sur les permis valides
            if (onlyValidLicenses)
            {
                query = query.Where(d => d.LicenseExpiryDate > DateTime.UtcNow);
            }

            // Filtre sur le type de permis
            if (!string.IsNullOrWhiteSpace(requiredLicenseType) &&
                Enum.TryParse<LicenseTypeEnums>(requiredLicenseType, out var licenseTypeEnum))
            {
                query = query.Where(d => d.LicenseType == licenseTypeEnum);
            }

            return await query
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des conducteurs disponibles");
            throw;
        }
    }

    public async Task<Driver> CreateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        try
        {
            driver.CreatedAt = DateTime.UtcNow;
            driver.UpdatedAt = DateTime.UtcNow;

            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Conducteur créé avec succès: {DriverName}", driver.FullName);
            return driver;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du conducteur");
            throw;
        }
    }

    public async Task<Driver> UpdateAsync(Driver driver, CancellationToken cancellationToken = default)
    {
        try
        {
            driver.UpdatedAt = DateTime.UtcNow;

            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Conducteur mis à jour avec succès: {DriverName}", driver.FullName);
            return driver;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du conducteur {Id}", driver.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var driver = await _context.Drivers.FindAsync(new object[] { id }, cancellationToken);
            if (driver == null)
                return false;

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Conducteur supprimé avec succès: {Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du conducteur {Id}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeDriverId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Drivers.Where(d => d.Email.ToLower() == email.ToLower());

            if (excludeDriverId.HasValue)
                query = query.Where(d => d.Id != excludeDriverId.Value);

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de l'unicité de l'email");
            throw;
        }
    }
    /// <inheritdoc/>
    public async Task<bool> IsLicenseNumberUniqueAsync(string licenseNumber, int? excludeDriverId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Drivers.Where(d => d.LicenseNumber.ToLower() == licenseNumber.ToLower());

            if (excludeDriverId.HasValue)
                query = query.Where(d => d.Id != excludeDriverId.Value);

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de l'unicité du numéro de permis");
            throw;
        }
    }
    /// <inheritdoc/>
    async Task<int> IDriverRepository.GetCountAsync(CancellationToken cancellationToken)
    {
        return await _context.Drivers.CountAsync(v => !v.IsDeleted, cancellationToken);
    }
}
