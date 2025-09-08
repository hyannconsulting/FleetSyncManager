using System.ComponentModel.DataAnnotations;

namespace Laroche.FleetManager.Application.DTOs
{
    /// <summary>
    /// DTO pour la création d'un véhicule
    /// </summary>
    public class CreateVehicleDto
    {
        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030)]
        public int Year { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentMileage { get; set; }

        public int? AssignedDriverId { get; set; }
        public string? Status { get; set; }
        public string FuelType { get; set; } = "0";
        public string? Vin { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un véhicule
    /// </summary>
    public class UpdateVehicleDto
    {
        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(1900, 2030)]
        public int Year { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentMileage { get; set; }

        public int? AssignedDriverId { get; set; }
        public string FuelType { get; set; } = "0";
        public string? Status { get; set; }
        public string? Vin { get; set; }
    }
}
