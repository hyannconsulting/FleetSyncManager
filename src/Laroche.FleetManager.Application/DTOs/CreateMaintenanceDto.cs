using System.ComponentModel.DataAnnotations;

namespace Laroche.FleetManager.Application.DTOs
{
    /// <summary>
    /// DTO pour la création d'une maintenance
    /// </summary>
    public class CreateMaintenanceDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'une maintenance
    /// </summary>
    public class UpdateMaintenanceDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int VehicleId { get; set; }
    }
}
