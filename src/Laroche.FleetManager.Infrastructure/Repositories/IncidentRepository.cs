using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Queries.Incidents;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Laroche.FleetManager.Infrastructure.Repositories;

/// <summary>
/// Provides methods for managing incidents in the application, including retrieving, adding, updating,  deleting, and
/// resolving incidents. This repository serves as an abstraction layer for interacting  with the underlying data store.
/// </summary>
/// <remarks>This repository is designed to handle operations related to the <see cref="Incident"/> entity, 
/// including support for pagination, filtering, and soft deletion. It ensures that only non-deleted  incidents are
/// returned by default and provides methods to mark incidents as resolved.</remarks>
public class IncidentRepository(ApplicationDbContext context) : IIncidentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Incident>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Incidents.Where(i => !i.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<IncidentDto>> GetPagedAsync(GetAllIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        var incidentsQuery = _context.Incidents.AsQueryable();
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            var search = query.SearchTerm.ToLower();
            incidentsQuery = incidentsQuery.Where(i =>
                i.Title.ToLower().Contains(search) ||
                i.Description.ToLower().Contains(search));
        }
        var totalCount = await incidentsQuery.CountAsync(cancellationToken);
        var items = await incidentsQuery
            .OrderByDescending(i => i.IncidentDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedResult<IncidentDto>
        {
            Items = items.Select(i => new IncidentDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                IncidentDate = i.IncidentDate,
                Status = i.IsResolved ? "Resolved" : "Open",
                VehicleId = i.VehicleId,
                DriverId = i.DriverId,
                IsResolved = i.IsResolved,
                ResolutionDate = i.ResolutionDate,
                EstimatedCost = i.EstimatedCost,
                ActualCost = i.ActualCost,
                Location = i.Location,
                IncidentType = string.Empty // à adapter si champ dans l'entité
            }),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    /// <inheritdoc/>
    public async Task<Incident?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Incidents.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);
    }

    public async Task<Incident> CreateAsync(Incident incident, CancellationToken cancellationToken = default)
    {
        incident.CreatedAt = DateTime.UtcNow;
        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task<Incident> UpdateAsync(Incident incident, CancellationToken cancellationToken = default)
    {
        incident.UpdatedAt = DateTime.UtcNow;
        _context.Incidents.Update(incident);
        await _context.SaveChangesAsync(cancellationToken);
        return incident;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var incident = await _context.Incidents.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);
        if (incident != null)
        {
            incident.IsDeleted = true;
            incident.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task MarkAsResolvedAsync(int id, CancellationToken cancellationToken = default)
    {
        var incident = await _context.Incidents.FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);
        if (incident != null)
        {
            incident.IsResolved = true;
            incident.ResolutionDate = DateTime.UtcNow;
            incident.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc/>
    async Task<bool> IIncidentRepository.ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Incidents.AnyAsync(i => i.Id == id && !i.IsDeleted, cancellationToken);
    }
    async Task<int> IIncidentRepository.GetCountAsync(CancellationToken cancellationToken)
    {
        return await _context.Incidents.CountAsync(v => !v.IsDeleted, cancellationToken);
    }

    async Task<IEnumerable<Incident>> IIncidentRepository.SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        return await _context.Incidents
            .Where(i => !i.IsDeleted &&
                       (i.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                        i.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)))
            .ToListAsync(cancellationToken);
    }
}
