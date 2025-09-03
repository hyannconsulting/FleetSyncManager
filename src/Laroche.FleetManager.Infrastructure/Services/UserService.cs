using Laroche.FleetManager.Application.Common;
using Laroche.FleetManager.Application.DTOs.Users;
using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service de gestion des utilisateurs
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Constructeur du service utilisateur
    /// </summary>
    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<ServiceResult> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Vérifier si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
            if (existingUser != null)
            {
                return ServiceResult.Failed("Un utilisateur avec cet email existe déjà.");
            }

            // Vérifier si le rôle existe
            if (!await _roleManager.RoleExistsAsync(createUserDto.Role))
            {
                return ServiceResult.Failed($"Le rôle '{createUserDto.Role}' n'existe pas.");
            }

            // Créer l'utilisateur
            var user = new ApplicationUser
            {
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                PhoneNumber = createUserDto.PhoneNumber,
                EmailConfirmed = true, // Auto-confirmer pour simplifier
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                // Assigner le rôle
                var roleResult = await _userManager.AddToRoleAsync(user, createUserDto.Role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Impossible d'assigner le rôle {Role} à l'utilisateur {Email}", createUserDto.Role, createUserDto.Email);
                    // On peut continuer, l'utilisateur est créé mais sans rôle
                }

                _logger.LogInformation("Utilisateur créé avec succès: {Email} avec le rôle {Role}", createUserDto.Email, createUserDto.Role);
                return ServiceResult.Success("Utilisateur créé avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors de la création de l'utilisateur: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de l'utilisateur {Email}", createUserDto.Email);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult> UpdateUserAsync(string userId, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Failed("Utilisateur introuvable.");
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Gestion du rôle si spécifié
                if (!string.IsNullOrEmpty(updateUserDto.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, updateUserDto.Role);
                }

                return ServiceResult.Success("Utilisateur mis à jour avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors de la mise à jour: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {UserId}", userId);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {UserId}", userId);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                AccessFailedCount = user.AccessFailedCount,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {Email}", email);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<PagedResult<UserDto>> GetUsersAsync(int page = 1, int pageSize = 10, string? searchTerm = null, string? role = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _userManager.Users.AsQueryable();

            // Filtre de recherche
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    u.Email!.Contains(searchTerm) ||
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                // Filtre par rôle si spécifié
                if (!string.IsNullOrEmpty(role) && !roles.Contains(role))
                    continue;

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    EmailConfirmed = user.EmailConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnd = user.LockoutEnd,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive
                });
            }

            return new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
            return new PagedResult<UserDto> { Items = [], TotalCount = 0, Page = page, PageSize = pageSize };
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Failed("Utilisateur introuvable.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("Utilisateur supprimé: {UserId} - {Email}", userId, user.Email);
                return ServiceResult.Success("Utilisateur supprimé avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors de la suppression: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur {UserId}", userId);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Failed("Utilisateur introuvable.");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return ServiceResult.Failed($"Le rôle '{role}' n'existe pas.");
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                return ServiceResult.Success($"Rôle '{role}' assigné avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors de l'assignation du rôle: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'assignation du rôle {Role} à l'utilisateur {UserId}", role, userId);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult> RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Failed("Utilisateur introuvable.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                return ServiceResult.Success($"Rôle '{role}' retiré avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors du retrait du rôle: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du retrait du rôle {Role} de l'utilisateur {UserId}", role, userId);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }

    /// <inheritdoc />
    public async Task<ServiceResult> SetUserActiveStatusAsync(string userId, bool isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Failed("Utilisateur introuvable.");
            }

            user.Status = isActive ? Laroche.FleetManager.Domain.Enums.UserStatus.Active : Laroche.FleetManager.Domain.Enums.UserStatus.Inactive;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var status = isActive ? "activé" : "désactivé";
                return ServiceResult.Success($"Utilisateur {status} avec succès.");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ServiceResult.Failed($"Erreur lors de la mise à jour du statut: {errors}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du statut de l'utilisateur {UserId}", userId);
            return ServiceResult.Failed("Une erreur interne s'est produite.");
        }
    }
}
