using Laroche.FleetManager.Application.Commands.Drivers;
using Laroche.FleetManager.Application.DTOs;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Provides functionality to interact with the Driver API, enabling operations such as retrieving, creating, and
/// updating driver data.
/// </summary>
/// <remarks>This service is designed to communicate with the Driver API at the endpoint <c>/api/v1/drivers</c>. 
/// It leverages HTTP client functionality to perform API requests and supports logging for diagnostic
/// purposes.</remarks>
public class DriverApiService : BaseApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>, IDriverApiService
{
    protected override string ApiEndpoint => "/api/v1/drivers";

    public DriverApiService(HttpClient httpClient, ILogger<DriverApiService> logger)
        : base(httpClient, logger)
    {
    }
}
