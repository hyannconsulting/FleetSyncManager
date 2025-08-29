using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Queries.Drivers;

namespace Laroche.FleetManager.Application.Handlers.Queries;

/// <summary>
/// Handler for GetAllDriversQuery
/// </summary>
public class GetDriversQueryHandler : IRequestHandler<GetAllDriversQuery, Result<PagedResult<DriverDto>>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDriversQueryHandler> _logger;

    public GetDriversQueryHandler(
        IDriverRepository driverRepository,
        IMapper mapper,
        ILogger<GetDriversQueryHandler> logger)
    {
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedResult<DriverDto>>> Handle(GetAllDriversQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Récupération des conducteurs - Page: {Page}, Taille: {PageSize}", request.Page, request.PageSize);

            // Validation des paramètres
            if (request.Page < 1)
                return Result<PagedResult<DriverDto>>.Failure("Le numéro de page doit être supérieur à 0");

            if (request.PageSize < 1 || request.PageSize > 100)
                return Result<PagedResult<DriverDto>>.Failure("La taille de page doit être entre 1 et 100");

            // Récupération des données depuis le repository
            var drivers = await _driverRepository.GetAllAsync(
                page: request.Page,
                pageSize: request.PageSize,
                searchTerm: request.SearchTerm,
                status: request.Status,
                licenseType: request.LicenseType,
                cancellationToken: cancellationToken);

            // Mapping vers DTOs
            var driverDtos = _mapper.Map<IReadOnlyList<DriverDto>>(drivers.Items);

            var result = new PagedResult<DriverDto>
            {
                Items = driverDtos,
                Page = drivers.Page,
                PageSize = drivers.PageSize,
                TotalCount = drivers.TotalCount
            };

            _logger.LogInformation("Récupération réussie de {Count} conducteurs", driverDtos.Count);

            return Result<PagedResult<DriverDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des conducteurs");
            return Result<PagedResult<DriverDto>>.Failure($"Erreur lors de la récupération des conducteurs: {ex.Message}");
        }
    }
}
