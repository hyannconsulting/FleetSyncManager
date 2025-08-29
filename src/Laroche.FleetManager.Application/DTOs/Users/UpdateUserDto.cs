using System.ComponentModel.DataAnnotations;

namespace Laroche.FleetManager.Application.DTOs.Users;

/// <summary>
/// DTO pour la mise à jour d'un utilisateur
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// Prénom de l'utilisateur
    /// </summary>
    [Required(ErrorMessage = "Le prénom est obligatoire")]
    [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nom de famille de l'utilisateur
    /// </summary>
    [Required(ErrorMessage = "Le nom est obligatoire")]
    [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Numéro de téléphone
    /// </summary>
    [Phone(ErrorMessage = "Format de numéro de téléphone invalide")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Rôle de l'utilisateur
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Statut d'activation
    /// </summary>
    public bool IsActive { get; set; } = true;
}
