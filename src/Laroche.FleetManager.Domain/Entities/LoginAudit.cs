using Laroche.FleetManager.Domain.Common;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Domain.Entities;

/// <summary>
/// Audit des connexions utilisateur
/// Trace toutes les tentatives de connexion pour la sécurité
/// </summary>
public class LoginAudit : BaseEntity
{
    /// <summary>
    /// ID de l'utilisateur qui tente de se connecter (nullable si utilisateur inexistant)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Email/username saisi lors de la tentative
    /// </summary>
    public string EmailAttempted { get; set; } = string.Empty;

    /// <summary>
    /// Nom d'utilisateur / email utilisé pour la connexion (legacy - utiliser EmailAttempted)
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// ID unique de session généré lors d'une connexion réussie
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Adresse IP de la tentative de connexion
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// User Agent du navigateur
    /// </summary>
    public string UserAgent { get; set; } = string.Empty;

    /// <summary>
    /// Résultat de la tentative de connexion
    /// </summary>
    public LoginResult Result { get; set; }

    /// <summary>
    /// Message détaillé du résultat (ex: raison de l'échec)
    /// </summary>
    public string? ResultMessage { get; set; }

    /// <summary>
    /// Date et heure de la tentative
    /// </summary>
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Durée de la session en minutes (si connexion réussie)
    /// </summary>
    public int? SessionDuration { get; set; }

    /// <summary>
    /// Date et heure de fin de session
    /// </summary>
    public DateTime? SessionEndAt { get; set; }

    /// <summary>
    /// Date de fin de session (déconnexion)
    /// </summary>
    public DateTime? SessionEndedAt { get; set; }

    /// <summary>
    /// Référence vers l'utilisateur
    /// </summary>
    public virtual ApplicationUser? User { get; set; }

    /// <summary>
    /// Localisation géographique approximative (optionnel)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Indique si cette connexion a été marquée comme suspecte
    /// </summary>
    public bool IsSuspicious { get; set; } = false;
}
