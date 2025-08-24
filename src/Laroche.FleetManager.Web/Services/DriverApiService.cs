using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Commands.Drivers;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Interface pour le service client API des conducteurs
/// </summary>
public interface IDriverApiService : IApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>
{
}

/// <summary>
/// Service client API pour les conducteurs
/// </summary>
public class DriverApiService : BaseApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>, IDriverApiService
{
    protected override string ApiEndpoint => "/api/v1/drivers";

    public DriverApiService(HttpClient httpClient, ILogger<DriverApiService> logger) 
        : base(httpClient, logger)
    {
    }
}
