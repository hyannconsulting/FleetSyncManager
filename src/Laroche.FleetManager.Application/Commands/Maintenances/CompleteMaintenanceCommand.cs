using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs;
using MediatR;

namespace Laroche.FleetManager.Application.Commands.Maintenances
{
    /// <summary>
    /// Command to complete a maintenance
    /// </summary>
    public class CompleteMaintenanceCommand : IRequest<Result<MaintenanceDto>>
    {
        /// <summary>
        /// Maintenance ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Completion date
        /// </summary>
        public DateTime CompletedDate { get; set; }

        /// <summary>
        /// Actual cost
        /// </summary>
        public decimal? ActualCost { get; set; }

        /// <summary>
        /// Vehicle mileage when completed
        /// </summary>
        public int? CompletionMileage { get; set; }

        /// <summary>
        /// Completion notes
        /// </summary>
        public string? CompletionNotes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompleteMaintenanceCommand"/> class with the specified
        /// maintenance ID.
        /// </summary>
        /// <remarks>The <see cref="CompletedDate"/> property is automatically set to the current date and
        /// time when the instance is created.</remarks>
        /// <param name="id">The unique identifier of the maintenance task to be marked as complete.</param>
        public CompleteMaintenanceCommand(int id)
        {
            Id = id;
            CompletedDate = DateTime.Now;
        }
    }
}
