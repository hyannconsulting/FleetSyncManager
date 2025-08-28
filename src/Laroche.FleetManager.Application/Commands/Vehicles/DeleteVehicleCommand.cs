using Laroche.FleetManager.Application.Common;
using MediatR;

namespace Laroche.FleetManager.Application.Commands.Vehicles;

/// <summary>
/// Command to delete a vehicle
/// </summary>
public class DeleteVehicleCommand : IRequest<Result>
{
    /// <summary>
    /// Vehicle ID to delete
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Initializes delete command
    /// </summary>
    /// <param name="id">Vehicle ID</param>
    public DeleteVehicleCommand(int id)
    {
        Id = id;
    }
}
