
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Queries.Incidents;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Infrastructure.Services;

public class IncidentService(IIncidentRepository incidentRepository) : IIncidentService
{
    private readonly IIncidentRepository _incidentRepository = incidentRepository;

    public async Task<PagedResult<IncidentDto>> GetPagedAsync(GetAllIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        return await _incidentRepository.GetPagedAsync(query, cancellationToken);
    }

    public async Task<IEnumerable<IncidentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var incidents = await _incidentRepository.GetAllAsync(cancellationToken);
        return incidents.Select(i => new IncidentDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            IncidentDate = i.IncidentDate,
            //  Status = i.s,
            VehicleId = i.VehicleId,
            DriverId = i.DriverId
        });
    }

    public async Task<IncidentDto?> GetIncidentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(id, cancellationToken);
        if (incident == null) return null;
        return new IncidentDto
        {
            Id = incident.Id,
            Title = incident.Title,
            Description = incident.Description,
            IncidentDate = incident.IncidentDate,
            //Status = incident.Status,
            VehicleId = incident.VehicleId,
            DriverId = incident.DriverId
        };
    }

    public async Task<IncidentDto> CreateIncidentAsync(CreateIncidentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new Incident
        {
            Title = dto.Title,
            Description = dto.Description,
            IncidentDate = dto.IncidentDate,
            //Status = dto.Status,
            VehicleId = dto.VehicleId,
            DriverId = dto.DriverId
        };
        var created = await _incidentRepository.CreateAsync(entity);

        return new IncidentDto
        {
            Id = created.Id,
            Title = created.Title,
            Description = created.Description,
            IncidentDate = created.IncidentDate,
            //Status = created.Status,
            VehicleId = created.VehicleId,
            DriverId = created.DriverId
        };
    }

    public async Task<IncidentDto> UpdateIncidentAsync(int id, UpdateIncidentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _incidentRepository.GetByIdAsync(id, cancellationToken) ?? throw new Exception("Incident non trouvé");
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.IncidentDate = dto.IncidentDate;
        //entity.Status = dto.Status;
        entity.VehicleId = dto.VehicleId;
        entity.DriverId = dto.DriverId;
        await _incidentRepository.UpdateAsync(entity);

        return new IncidentDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IncidentDate = entity.IncidentDate,
            //Status = entity.Status,
            VehicleId = entity.VehicleId,
            DriverId = entity.DriverId
        };
    }

    public async Task DeleteIncidentAsync(int id, CancellationToken cancellationToken = default)
    {
        await _incidentRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task MarkAsResolvedAsync(int id, CancellationToken cancellationToken = default)
    {
        await _incidentRepository.MarkAsResolvedAsync(id, cancellationToken);
    }

    async Task<int> IIncidentService.GetCountAsync(CancellationToken cancellationToken)
    {
        return await _incidentRepository.GetCountAsync(cancellationToken);
    }
}
