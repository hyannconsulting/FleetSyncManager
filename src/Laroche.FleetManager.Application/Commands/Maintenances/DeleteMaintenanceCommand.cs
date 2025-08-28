using Laroche.FleetManager.Application.Common;
using MediatR;

namespace Laroche.FleetManager.Application.Commands.Maintenances;

/// <summary>
/// Command to delete a maintenance record
/// </summary>
public class DeleteMaintenanceCommand : IRequest<Result>
{
    /// <summary>
    /// Maintenance ID to delete
    /// </summary>
    public int Id { get; set; }

    public DeleteMaintenanceCommand(int id)
    {
        Id = id;
    }
}
