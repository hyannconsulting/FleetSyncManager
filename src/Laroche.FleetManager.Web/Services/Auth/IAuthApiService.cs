using Laroche.FleetManager.Web.Models.Auth;

namespace Laroche.FleetManager.Web.Services.Auth;

/// <summary>
/// Interface pour les services d'authentification via API
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Authentifie un utilisateur
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="password">Mot de passe</param>
    /// <returns>Résultat de l'authentification</returns>
    Task<LoginResult> LoginAsync(string email, string password);

    /// <summary>
    /// Déconnecte l'utilisateur actuel
    /// </summary>
    /// <returns>Résultat de la déconnexion</returns>
    Task<bool> LogoutAsync();

    /// <summary>
    /// Crée un nouveau compte utilisateur
    /// </summary>
    /// <param name="model">Données du nouveau compte</param>
    /// <returns>Résultat de la création</returns>
    Task<RegisterResult> RegisterAsync(RegisterModel model);

    /// <summary>
    /// Récupère les informations de l'utilisateur connecté
    /// </summary>
    /// <returns>Informations utilisateur ou null si non connecté</returns>
    Task<UserInfoResult?> GetCurrentUserAsync();

    /// <summary>
    /// Envoie un email de réinitialisation du mot de passe
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <returns>Résultat de l'opération</returns>
    Task<bool> ForgotPasswordAsync(string email);

    /// <summary>
    /// Réinitialise le mot de passe avec un token
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="token">Token de réinitialisation</param>
    /// <param name="newPassword">Nouveau mot de passe</param>
    /// <returns>Résultat de l'opération</returns>
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    /// <summary>
    /// Change le mot de passe de l'utilisateur connecté
    /// </summary>
    /// <param name="currentPassword">Mot de passe actuel</param>
    /// <param name="newPassword">Nouveau mot de passe</param>
    /// <returns>Résultat de l'opération</returns>
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
}
