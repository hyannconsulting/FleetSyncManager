using Laroche.FleetManager.Application.DTOs;
using Laroche.FleetManager.Application.Commands.Drivers;

namespace Laroche.FleetManager.Web.Services;

/// <summary>
/// Interface pour le service client API des conducteurs
/// </summary>
public interface IDriverApiService : IApiClientService<DriverDto, CreateDriverCommand, UpdateDriverCommand>
{
}
