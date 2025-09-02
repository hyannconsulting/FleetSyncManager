using Laroche.FleetManager.Application.Models;
using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Interfaces;

/// <summary>
/// Service d'authentification et de gestion des utilisateurs selon TASK-002
/// Gère l'authentification avec 3 rôles (Admin, FleetManager, Driver), 
/// sessions de 30 minutes, verrouillage après 5 tentatives échouées
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authentifie un utilisateur avec email et mot de passe
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="password">Mot de passe</param>
    /// <param name="rememberMe">Se souvenir de la connexion</param>
    /// <param name="ipAddress">Adresse IP de la requête</param>
    /// <param name="userAgent">User-Agent du navigateur</param>
    /// <returns>Résultat de l'authentification avec session de 30min</returns>
    Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe,
        string ipAddress, string userAgent);

    /// <summary>
    /// Déconnecte un utilisateur et termine la session
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="sessionId">ID de session pour audit trail</param>
    /// <returns>True si déconnexion réussie</returns>
    Task<bool> LogoutAsync(string userId, string? sessionId = null);

    /// <summary>
    /// Crée un nouvel utilisateur avec un des 3 rôles système
    /// </summary>
    /// <param name="email">Email unique</param>
    /// <param name="password">Mot de passe respectant les politiques</param>
    /// <param name="firstName">Prénom</param>
    /// <param name="lastName">Nom</param>
    /// <param name="telePhone"></param>
    /// <param name="role">Rôle: Admin, FleetManager ou Driver</param>
    /// <returns>Résultat de la création utilisateur</returns>
    Task<CreateUserResult> CreateUserAsync(string email, string password, string firstName,
        string lastName, string telePhone, string role);

    /// <summary>
    /// Démarre le processus de réinitialisation de mot de passe
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <returns>True si token envoyé (ne révèle pas si email existe)</returns>
    Task<bool> ResetPasswordAsync(string email);

    /// <summary>
    /// Confirme la réinitialisation avec token de sécurité
    /// </summary>
    /// <param name="email">Email utilisateur</param>
    /// <param name="token">Token de réinitialisation</param>
    /// <param name="newPassword">Nouveau mot de passe</param>
    /// <returns>True si réinitialisation réussie</returns>
    Task<bool> ConfirmResetPasswordAsync(string email, string token, string newPassword);

    /// <summary>
    /// Change le mot de passe utilisateur avec vérification de l'ancien
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="currentPassword">Mot de passe actuel</param>
    /// <param name="newPassword">Nouveau mot de passe</param>
    /// <returns>True si changement réussi</returns>
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);

    /// <summary>
    /// Verrouille un compte utilisateur (Admin uniquement)
    /// </summary>
    /// <param name="userId">ID utilisateur à verrouiller</param>
    /// <param name="lockoutEnd">Fin du verrouillage (null = permanent)</param>
    /// <returns>True si verrouillage appliqué</returns>
    Task<bool> LockUserAsync(string userId, DateTime? lockoutEnd = null);

    /// <summary>
    /// Déverrouille un compte utilisateur (Admin uniquement)
    /// </summary>
    /// <param name="userId">ID utilisateur à déverrouiller</param>
    /// <returns>True si déverrouillage réussi</returns>
    Task<bool> UnlockUserAsync(string userId);

    /// <summary>
    /// Récupère un utilisateur par ID avec ses rôles
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <returns>Utilisateur ou null si introuvable</returns>
    Task<ApplicationUser?> GetUserByIdAsync(string userId);

    /// <summary>
    /// Récupère un utilisateur par email avec ses rôles
    /// </summary>
    /// <param name="email">Email utilisateur</param>
    /// <returns>Utilisateur ou null si introuvable</returns>
    Task<ApplicationUser?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Vérifie si utilisateur a un rôle spécifique
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="role">Rôle à vérifier (Admin/FleetManager/Driver)</param>
    /// <returns>True si utilisateur a le rôle</returns>
    Task<bool> IsUserInRoleAsync(string userId, string role);

    /// <summary>
    /// Ajoute un rôle à un utilisateur (Admin uniquement)
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="role">Rôle à ajouter</param>
    /// <returns>True si rôle ajouté</returns>
    Task<bool> AddUserToRoleAsync(string userId, string role);

    /// <summary>
    /// Retire un rôle d'un utilisateur (Admin uniquement)
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="role">Rôle à retirer</param>
    /// <returns>True si rôle retiré</returns>
    Task<bool> RemoveUserFromRoleAsync(string userId, string role);

    /// <summary>
    /// Récupère tous les rôles d'un utilisateur
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <returns>Liste des rôles</returns>
    Task<IList<string>> GetUserRolesAsync(string userId);

    /// <summary>
    /// Valide si session utilisateur est encore active (30min max)
    /// </summary>
    /// <param name="userId">ID utilisateur</param>
    /// <param name="sessionId">ID de session</param>
    /// <returns>True si session valide</returns>
    Task<bool> IsSessionValidAsync(string userId, string sessionId);
}
