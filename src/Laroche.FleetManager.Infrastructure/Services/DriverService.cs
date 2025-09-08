using AutoMapper;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service pour la gestion des conducteurs (CRUD, pagination, etc.)
/// </summary>
public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public DriverService(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Récupère tous les conducteurs.
    /// </summary>
    public async Task<IEnumerable<DriverDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var drivers = await _driverRepository.GetAllAsync(cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<DriverDto>>(drivers.Items);
    }

    /// <summary>
    /// Récupère un conducteur par son identifiant.
    /// </summary>
    public async Task<DriverDto?> GetDriverByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(id, cancellationToken);
        return driver == null ? null : _mapper.Map<DriverDto>(driver);
    }

    /// <summary>
    /// Crée un nouveau conducteur.
    /// </summary>
    public async Task<DriverDto> CreateDriverAsync(CreateDriverDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var driver = _mapper.Map<Driver>(dto);
        var createdDriver = await _driverRepository.CreateAsync(driver, cancellationToken);
        return _mapper.Map<DriverDto>(createdDriver);
    }

    /// <summary>
    /// Met à jour un conducteur existant.
    /// </summary>
    public async Task<DriverDto> UpdateDriverAsync(int id, UpdateDriverDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var existingDriver = await _driverRepository.GetByIdAsync(id, cancellationToken);
        if (existingDriver == null)
            throw new InvalidOperationException($"Conducteur avec l'ID {id} non trouvé.");

        _mapper.Map(dto, existingDriver);
        var updatedDriver = await _driverRepository.UpdateAsync(existingDriver, cancellationToken);
        return _mapper.Map<DriverDto>(updatedDriver);
    }

    /// <summary>
    /// Supprime un conducteur par son identifiant.
    /// </summary>
    public async Task DeleteDriverAsync(int id, CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(id, cancellationToken);
        if (driver == null)
            throw new InvalidOperationException($"Conducteur avec l'ID {id} non trouvé.");

        await _driverRepository.DeleteAsync(id, cancellationToken);
    }

    /// <summary>
    /// Récupère le nombre total de conducteurs.
    /// </summary>
    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _driverRepository.GetCountAsync(cancellationToken);
    }
}
