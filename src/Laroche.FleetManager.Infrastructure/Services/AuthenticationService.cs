using Laroche.FleetManager.Application.Interfaces;
using Laroche.FleetManager.Application.Models;
using Laroche.FleetManager.Domain.Constants;
using Laroche.FleetManager.Domain.Entities;
using Laroche.FleetManager.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Laroche.FleetManager.Infrastructure.Services;

/// <summary>
/// Service d'authentification implémentant les spécifications TASK-002
/// Sessions de 30 minutes, 3 rôles système, verrouillage après 5 échecs
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILoginAuditService _loginAuditService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenProvider _tokenProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class, providing services for user
    /// authentication, role management, and token generation.
    /// </summary>
    /// <param name="userManager">The <see cref="UserManager{TUser}"/> instance used to manage user accounts.</param>
    /// <param name="signInManager">The <see cref="SignInManager{TUser}"/> instance used to handle user sign-in operations.</param>
    /// <param name="roleManager">The <see cref="RoleManager{TRole}"/> instance used to manage user roles.</param>
    /// <param name="loginAuditService">The service responsible for auditing login attempts and related activities.</param>
    /// <param name="logger">The <see cref="ILogger{TCategoryName}"/> instance used for logging authentication-related events.</param>
    /// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> instance used to access the current HTTP context.</param>
    /// <param name="tokenProvider">The service responsible for generating and validating authentication tokens.</param>
    /// <exception cref="ArgumentNullException">Thrown if any of the provided dependencies are <see langword="null"/>.</exception>
    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ILoginAuditService loginAuditService,
        ILogger<AuthenticationService> logger,
        IHttpContextAccessor httpContextAccessor,
        ITokenProvider tokenProvider)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _loginAuditService = loginAuditService ?? throw new ArgumentNullException(nameof(loginAuditService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
    }

    public async Task<AuthenticationResult> LoginAsync(string email, string password, bool rememberMe,
        string ipAddress, string userAgent)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await _loginAuditService.LogLoginAttemptAsync(null, email, LoginResult.InvalidCredentials,
                ipAddress, userAgent);
            return AuthenticationResult.Failure("Email et mot de passe requis");
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            // Vérifier si utilisateur existe
            if (user == null)
            {
                await _loginAuditService.LogLoginAttemptAsync(null, email, LoginResult.UserNotFound,
                    ipAddress, userAgent);
                return AuthenticationResult.Failure("Identifiants invalides");
            }

            // Vérifier statut utilisateur
            if (user.Status != UserStatus.Active)
            {
                await _loginAuditService.LogLoginAttemptAsync(user.Id, email, LoginResult.AccountDisabled,
                    ipAddress, userAgent);
                return AuthenticationResult.Failure("Compte désactivé");
            }

            // Vérifier verrouillage
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _loginAuditService.LogLoginAttemptAsync(user.Id, email, LoginResult.AccountLocked,
                    ipAddress, userAgent);
                return AuthenticationResult.Failure("Compte temporairement verrouillé");
            }

            // Vérifier si trop d'échecs récents (5 max selon TASK-002)
            if (await _loginAuditService.HasExceededFailedAttemptsAsync(user.Id, TimeSpan.FromMinutes(15), 5))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(30));
                await _loginAuditService.LogLoginAttemptAsync(user.Id, email, LoginResult.TooManyAttempts,
                    ipAddress, userAgent);
                return AuthenticationResult.Failure("Trop de tentatives échouées. Compte verrouillé temporairement");
            }

            // Vérifier le mot de passe manuellement
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);

            if (passwordValid)
            {
                // Tentative de connexion avec SignInManager seulement si HttpContext est disponible
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    try
                    {
                        await _signInManager.SignInAsync(user, rememberMe);
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("HttpContext"))
                    {
                        // Si SignInManager échoue à cause du HttpContext, continuer sans lui
                        _logger.LogWarning("SignInManager échoué à cause de HttpContext null, continuation sans cookie auth");
                    }
                }
                else
                {
                    _logger.LogWarning("HttpContext non disponible, connexion sans cookie auth");
                }

                // Mise à jour des informations de connexion
                user.LastLoginAt = DateTime.UtcNow;
                user.LastLoginIp = ipAddress;
                user.FailedLoginAttempts = 0; // Reset compteur échecs
                await _userManager.UpdateAsync(user);

                // Générer ID de session pour tracking
                var sessionId = Guid.NewGuid().ToString();

                //// Détecter activité suspecte
                //var isSuspicious = await _loginAuditService.DetectSuspiciousActivityAsync(user.Id, ipAddress);

                // Logger la connexion réussie
                var loginAudit = await _loginAuditService.LogLoginAttemptAsync(user.Id, email,
                    LoginResult.Success, ipAddress, userAgent, sessionId);

                //if (isSuspicious)
                //{
                //    await _loginAuditService.MarkAsSuspiciousAsync(loginAudit.Id, "Connexion depuis nouvelle IP ou comportement inhabituel");
                //}

                var userRoles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("Connexion réussie pour l'utilisateur {Email} depuis {IpAddress}", email, ipAddress);

                var loginUser = new Application.DTOs.Users.UserDto(user, userRoles);

                return AuthenticationResult.Success(
                   _tokenProvider.CreateToken(loginUser),
                    sessionId,
                    TimeSpan.FromMinutes(30)); // Session 30min selon TASK-002
            }
            else
            {
                // Mot de passe invalide
                user.FailedLoginAttempts++;

                // Vérifier si on doit verrouiller après cet échec
                if (user.FailedLoginAttempts >= 5)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(30));
                    await _userManager.UpdateAsync(user);

                    await _loginAuditService.LogLoginAttemptAsync(user.Id, email, LoginResult.AccountLocked,
                        ipAddress, userAgent);
                    return AuthenticationResult.Failure("Compte verrouillé suite à trop d'échecs");
                }
                else
                {
                    await _userManager.UpdateAsync(user);

                    await _loginAuditService.LogLoginAttemptAsync(user.Id, email, LoginResult.InvalidCredentials,
                        ipAddress, userAgent);
                    return AuthenticationResult.Failure("Identifiants invalides");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'authentification pour {Email}", email);
            return AuthenticationResult.Failure("Erreur interne lors de l'authentification");
        }
    }

    public async Task<bool> LogoutAsync(string userId, string? sessionId = null)
    {
        try
        {
            // Seulement utiliser SignInManager si HttpContext est disponible
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                try
                {
                    //var user = httpContext.User;
                    //var userEmail = user?.Identity?.Name ?? "Unknown";
                    //var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                    //var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                    //await _loginAuditService.LogLogoutAsync(LogLogoutAsync
                    await _signInManager.SignOutAsync();
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("HttpContext"))
                {
                    _logger.LogWarning("SignOutAsync échoué à cause de HttpContext null");
                }
            }

            if (!string.IsNullOrEmpty(sessionId))
            {
                await _loginAuditService.LogSessionEndAsync(sessionId, "Logout");
            }

            _logger.LogInformation("Déconnexion de l'utilisateur {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion pour {UserId}", userId);
            return false;
        }
    }

    public async Task<CreateUserResult> CreateUserAsync(string email, string password, string firstName,
        string lastName, string telePhone, string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return CreateUserResult.Failure("Email et mot de passe requis");
        }

        if (!IsValidRole(role))
        {
            return CreateUserResult.Failure($"Rôle invalide. Rôles autorisés: {UserRoles.Admin}, {UserRoles.FleetManager}, {UserRoles.Driver}");
        }

        try
        {
            // Vérifier si email existe déjà
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return CreateUserResult.Failure("Un utilisateur avec cet email existe déjà");
            }

            // Créer nouvel utilisateur
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true, // Auto-confirmer pour simplifier
                FirstName = firstName,
                LastName = lastName,
                Status = UserStatus.Active,
                MustChangePassword = true // Forcer changement au premier login
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Ajouter le rôle
                await _userManager.AddToRoleAsync(user, role);

                _logger.LogInformation("Utilisateur créé avec succès: {Email} avec le rôle {Role}", email, role);

                return CreateUserResult.Success(user.Id, $"Utilisateur {email} créé avec le rôle {role}");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return CreateUserResult.Failure($"Erreur création utilisateur: {errors}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création d'utilisateur {Email}", email);
            return CreateUserResult.Failure("Erreur interne lors de la création");
        }
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Ne pas révéler si l'email existe (sécurité)
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Envoyer email avec token (intégration service email)
            // Pour l'instant, logger le token (développement uniquement)
            _logger.LogInformation("Token de réinitialisation généré pour {Email}: {Token}", email, token);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la réinitialisation pour {Email}", email);
            return false;
        }
    }

    public async Task<bool> ConfirmResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Mot de passe réinitialisé avec succès pour {Email}", email);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la confirmation de réinitialisation pour {Email}", email);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Mot de passe changé avec succès pour l'utilisateur {UserId}", userId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du changement de mot de passe pour {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> LockUserAsync(string userId, DateTime? lockoutEnd = null)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var lockoutEndDate = lockoutEnd ?? DateTimeOffset.MaxValue; // Permanent si pas spécifié
            await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);

            // Forcer déconnexion de toutes les sessions
            await _loginAuditService.ForceLogoutAllSessionsAsync(userId, "Account locked by admin");

            _logger.LogInformation("Utilisateur {UserId} verrouillé jusqu'au {LockoutEnd}", userId, lockoutEndDate);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du verrouillage de {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UnlockUserAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            user.FailedLoginAttempts = 0; // Reset compteur
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Utilisateur {UserId} déverrouillé", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du déverrouillage de {UserId}", userId);
            return false;
        }
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        try
        {
            return await _userManager.FindByIdAsync(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {UserId}", userId);
            return null;
        }
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        try
        {
            return await _userManager.FindByEmailAsync(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {Email}", email);
            return null;
        }
    }

    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de rôle pour {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> AddUserToRoleAsync(string userId, string role)
    {
        if (!IsValidRole(role))
        {
            return false;
        }

        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Rôle {Role} ajouté à l'utilisateur {UserId}", role, userId);
            }

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'ajout de rôle pour {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> RemoveUserFromRoleAsync(string userId, string role)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (result.Succeeded)
            {
                _logger.LogInformation("Rôle {Role} retiré de l'utilisateur {UserId}", role, userId);
            }

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du retrait de rôle pour {UserId}", userId);
            return false;
        }
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des rôles pour {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<bool> IsSessionValidAsync(string userId, string sessionId)
    {
        try
        {
            // Vérifier si session existe et est dans la limite de 30 minutes
            var activeSessions = await _loginAuditService.GetActiveSessionsAsync(TimeSpan.FromMinutes(30));
            return activeSessions.Any(s => s.UserId == userId && s.SessionId == sessionId && s.SessionEndAt == null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation de session pour {UserId}", userId);
            return false;
        }
    }

    private static bool IsValidRole(string role)
    {
        return role == UserRoles.Admin || role == UserRoles.FleetManager || role == UserRoles.Driver;
    }
}
