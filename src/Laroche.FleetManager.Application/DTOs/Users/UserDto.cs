namespace Laroche.FleetManager.Application.DTOs.Users;

/// <summary>
/// DTO représentant un utilisateur
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identifiant unique de l'utilisateur
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email de l'utilisateur
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'utilisateur
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Prénom de l'utilisateur
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nom de famille de l'utilisateur
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Numéro de téléphone
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Indique si le numéro de téléphone est confirmé
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Indique si l'email est confirmé
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Indique si l'authentification à deux facteurs est activée
    /// </summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>
    /// Date de fin de verrouillage si l'utilisateur est verrouillé
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// Indique si le verrouillage est activé pour cet utilisateur
    /// </summary>
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// Nombre d'échecs d'accès
    /// </summary>
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// Rôles assignés à l'utilisateur
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Date de dernière connexion
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Indique si l'utilisateur est actif
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indique si l'utilisateur est en ligne
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Indique si l'utilisateur est verrouillé
    /// </summary>
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
}
