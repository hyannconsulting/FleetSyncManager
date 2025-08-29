namespace Laroche.FleetManager.Domain.Enums;

/// <summary>
/// Statut d'un utilisateur dans le système
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Utilisateur actif
    /// </summary>
    Active = 1,

    /// <summary>
    /// Utilisateur inactif (temporairement désactivé)
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Utilisateur suspendu (sanction disciplinaire)
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Utilisateur verrouillé (tentatives de connexion échouées)
    /// </summary>
    Locked = 4,

    /// <summary>
    /// Utilisateur archivé (ne travaille plus dans l'organisation)
    /// </summary>
    Archived = 5
}
