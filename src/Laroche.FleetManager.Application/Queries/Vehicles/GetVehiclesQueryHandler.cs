using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Queries.Vehicles;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Enums;

// Alias pour simplifier l'usage
using VehicleStatus = Laroche.FleetManager.Domain.Enums.VehicleStatusEnums;

namespace Laroche.FleetManager.Application.Queries.Vehicles;

/// <summary>
/// Handler pour récupérer la liste paginée des véhicules
/// </summary>
public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, Result<PagedResult<VehicleDto>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetVehiclesQueryHandler> _logger;

    public GetVehiclesQueryHandler(
        IVehicleRepository vehicleRepository,
        IMapper mapper,
        ILogger<GetVehiclesQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PagedResult<VehicleDto>>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Récupération des véhicules - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

            // Parse des enum pour les filtres
            VehicleStatus? status = null;
            if (!string.IsNullOrEmpty(request.Status))
            {
                Enum.TryParse(request.Status, out VehicleStatus parsedStatus);
                status = parsedStatus;
            }

            FuelType? fuelType = null;
            if (!string.IsNullOrEmpty(request.FuelType))
            {
                Enum.TryParse(request.FuelType, out FuelType parsedFuelType);
                fuelType = parsedFuelType;
            }

            // Appel du repository
            var (vehicles, totalCount) = await _vehicleRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Brand,
                status,
                fuelType,
                request.SortBy,
                request.SortDirection,
                cancellationToken);

            // Mapping vers DTOs
            var vehicleDtos = _mapper.Map<IReadOnlyList<VehicleDto>>(vehicles.ToList());

            var pagedResult = PagedResult<VehicleDto>.Create(
                vehicleDtos,
                totalCount,
                request.Page,
                request.PageSize);

            _logger.LogInformation("Véhicules récupérés avec succès - Total: {TotalCount}, Page: {Page}/{TotalPages}", 
                totalCount, request.Page, pagedResult.TotalPages);

            return Result<PagedResult<VehicleDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des véhicules");
            return Result<PagedResult<VehicleDto>>.Failure($"Erreur lors de la récupération des véhicules: {ex.Message}");
        }
    }
}
