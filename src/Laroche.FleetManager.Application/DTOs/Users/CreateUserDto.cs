using System.ComponentModel.DataAnnotations;

namespace Laroche.FleetManager.Application.DTOs.Users;

/// <summary>
/// DTO pour la création d'un utilisateur
/// </summary>
public class CreateUserDto
{
    /// <summary>
    /// Adresse email de l'utilisateur
    /// </summary>
    [Required(ErrorMessage = "L'email est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string Email { get; set; } = string.Empty;

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
    [Required(ErrorMessage = "Le rôle est obligatoire")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Mot de passe
    /// </summary>
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Le mot de passe doit contenir entre 8 et 100 caractères")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
