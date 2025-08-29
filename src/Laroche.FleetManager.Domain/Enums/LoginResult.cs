namespace Laroche.FleetManager.Domain.Enums;

/// <summary>
/// Résultat d'une tentative de connexion
/// </summary>
public enum LoginResult
{
    /// <summary>
    /// Connexion réussie
    /// </summary>
    Success = 1,

    /// <summary>
    /// Échec - utilisateur non trouvé
    /// </summary>
    UserNotFound = 2,

    /// <summary>
    /// Échec - mot de passe incorrect / identifiants invalides
    /// </summary>
    InvalidCredentials = 3,

    /// <summary>
    /// Échec - compte verrouillé
    /// </summary>
    AccountLocked = 4,

    /// <summary>
    /// Échec - compte désactivé
    /// </summary>
    AccountDisabled = 5,

    /// <summary>
    /// Échec - email non confirmé
    /// </summary>
    EmailNotConfirmed = 6,

    /// <summary>
    /// Échec - authentification à deux facteurs requise
    /// </summary>
    RequiresTwoFactor = 7,

    /// <summary>
    /// Échec - changement de mot de passe requis
    /// </summary>
    PasswordChangeRequired = 8,

    /// <summary>
    /// Échec - tentative suspecte bloquée
    /// </summary>
    SuspiciousActivity = 9,

    /// <summary>
    /// Échec - trop de tentatives échouées
    /// </summary>
    TooManyAttempts = 10,

    /// <summary>
    /// Erreur système
    /// </summary>
    SystemError = 11,

    /// <summary>
    /// Déconnexion réussie
    /// </summary>
    Logout = 12
}
