using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs.Users;

namespace Laroche.FleetManager.Application.Interfaces;

/// <summary>
/// Interface pour la gestion des utilisateurs
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    /// <param name="createUserDto">Données de création de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Met à jour un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="updateUserDto">Données de mise à jour</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un utilisateur par son ID
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>DTO de l'utilisateur</returns>
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère un utilisateur par son email
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>DTO de l'utilisateur</returns>
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Récupère tous les utilisateurs avec pagination
    /// </summary>
    /// <param name="page">Numéro de page</param>
    /// <param name="pageSize">Taille de la page</param>
    /// <param name="searchTerm">Terme de recherche</param>
    /// <param name="role">Filtre par rôle</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Liste paginée d'utilisateurs</returns>
    Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? role = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supprime un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigne un rôle à un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="role">Nom du rôle</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retire un rôle à un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="role">Nom du rôle</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Active ou désactive un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <param name="isActive">État d'activation</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération</returns>
    Task<ServiceResult> SetUserActiveStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default);
}
