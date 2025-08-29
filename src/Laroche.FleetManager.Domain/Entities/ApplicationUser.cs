using Microsoft.AspNetCore.Identity;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Utilisateur de l'application FleetSyncManager
/// Étend IdentityUser pour ajouter des propriétés métier
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Prénom de l'utilisateur
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nom de famille de l'utilisateur
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Nom complet (prénom + nom)
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Statut de l'utilisateur
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// Indique si l'utilisateur est actif
    /// </summary>
    public bool IsActive => Status == UserStatus.Active;

    /// <summary>
    /// Date de création du compte
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière modification
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Date de dernière connexion
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Adresse IP de dernière connexion
    /// </summary>
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// Nombre de tentatives de connexion échouées consécutives
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Date de verrouillage du compte (si applicable)
    /// </summary>
    public DateTime? LockoutEndDateUtc { get; set; }

    /// <summary>
    /// Indique si l'utilisateur doit changer son mot de passe à la prochaine connexion
    /// </summary>
    public bool MustChangePassword { get; set; } = false;

    /// <summary>
    /// Notes administratives sur l'utilisateur
    /// </summary>
    public string? AdminNotes { get; set; }

    /// <summary>
    /// ID du conducteur associé (si applicable)
    /// Utilisé pour lier un compte utilisateur à un profil conducteur
    /// </summary>
    public int? DriverId { get; set; }

    /// <summary>
    /// Conducteur associé à cet utilisateur
    /// </summary>
    public virtual Driver? Driver { get; set; }

    /// <summary>
    /// Historique des connexions de cet utilisateur
    /// </summary>
    public virtual ICollection<LoginAudit> LoginAudits { get; set; } = new List<LoginAudit>();
}
