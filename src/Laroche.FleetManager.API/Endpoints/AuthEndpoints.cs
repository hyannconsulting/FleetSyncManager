using Laroche.FleetManager.Application.Interfaces;
using System.Security.Claims;

namespace Laroche.FleetManager.API.Endpoints;

/// <summary>
/// Endpoints d'authentification pour l'API FleetSyncManager
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authGroup = endpoints.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // POST /api/v1/auth/login
        authGroup.MapPost("/login", async (
            LoginRequest request,
            IAuthenticationService authService,
            ILogger<Program> logger,
            HttpContext httpContext) =>
        {
            try
            {
                logger.LogInformation("Tentative de connexion pour l'utilisateur: {Email}", request.Email);

                var userAgent = httpContext.Request.Headers.UserAgent.ToString();
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                var result = await authService.LoginAsync(
                    request.Email,
                    request.Password,
                    request.RememberMe,
                    ipAddress,
                    userAgent);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Connexion réussie pour: {Email}", request.Email);
                    return Results.Ok(new LoginResponse
                    {
                        Success = true,
                        Message = "Connexion réussie",
                        User = result.User,
                        SessionId = result.SessionId,
                        SessionDuration = result.SessionDuration,
                        UserRoles = result.UserRoles
                    });
                }

                logger.LogWarning("Échec de connexion pour: {Email}, Raison: {Message}", request.Email, result.Message);
                return Results.BadRequest(new LoginResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la connexion pour: {Email}", request.Email);
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("Login")
        .WithSummary("Connexion utilisateur")
        .WithDescription("Authentifie un utilisateur avec email et mot de passe")
        .Produces<LoginResponse>(200)
        .Produces<LoginResponse>(400)
        .Produces(500);

        // POST /api/v1/auth/logout
        authGroup.MapPost("/logout", async (
            IAuthenticationService authService,
            ILogger<Program> logger,
            HttpContext httpContext,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var sessionId = user.FindFirst("SessionId")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.BadRequest(new { Message = "Utilisateur non authentifié" });
                }

                await authService.LogoutAsync(userId, sessionId);

                logger.LogInformation("Déconnexion réussie pour l'utilisateur: {UserId}", userId);

                return Results.Ok(new { Message = "Déconnexion réussie" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la déconnexion");
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("Logout")
        .WithSummary("Déconnexion utilisateur")
        .WithDescription("Déconnecte l'utilisateur actuellement connecté")
        .RequireAuthorization()
        .Produces(200)
        .Produces(400)
        .Produces(500);

        // POST /api/v1/auth/register
        authGroup.MapPost("/register", async (
            RegisterRequest request,
            IAuthenticationService authService,
            ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Tentative d'inscription pour: {Email}", request.Email);

                var result = await authService.CreateUserAsync(
                    request.Email,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber ?? string.Empty,
                    request.Role);

                if (result.IsSuccess)
                {
                    logger.LogInformation("Inscription réussie pour: {Email}", request.Email);
                    return Results.Ok(new RegisterResponse
                    {
                        Success = true,
                        Message = "Inscription réussie",
                        UserId = result.UserId
                    });
                }

                logger.LogWarning("Échec d'inscription pour: {Email}, Raison: {Message}", request.Email, result.Message);
                return Results.BadRequest(new RegisterResponse
                {
                    Success = false,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'inscription pour: {Email}", request.Email);
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("Register")
        .WithSummary("Inscription utilisateur")
        .WithDescription("Crée un nouveau compte utilisateur")
        .Produces<RegisterResponse>(200)
        .Produces<RegisterResponse>(400)
        .Produces(500);

        // POST /api/v1/auth/forgot-password
        authGroup.MapPost("/forgot-password", async (
            ForgotPasswordRequest request,
            IAuthenticationService authService,
            ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Demande de réinitialisation de mot de passe pour: {Email}", request.Email);

                var result = await authService.ResetPasswordAsync(request.Email);

                // Toujours retourner succès pour des raisons de sécurité
                // (ne pas révéler si l'email existe ou non)
                return Results.Ok(new
                {
                    Message = "Si cette adresse email est enregistrée, vous recevrez un lien de réinitialisation."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la demande de réinitialisation pour: {Email}", request.Email);
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("ForgotPassword")
        .WithSummary("Mot de passe oublié")
        .WithDescription("Envoie un email de réinitialisation de mot de passe")
        .Produces(200)
        .Produces(500);

        // POST /api/v1/auth/reset-password
        authGroup.MapPost("/reset-password", async (
            ResetPasswordRequest request,
            IAuthenticationService authService,
            ILogger<Program> logger) =>
        {
            try
            {
                var success = await authService.ConfirmResetPasswordAsync(
                    request.Email,
                    request.Token,
                    request.NewPassword);

                if (success)
                {
                    logger.LogInformation("Réinitialisation de mot de passe réussie pour: {Email}", request.Email);
                    return Results.Ok(new { Message = "Mot de passe réinitialisé avec succès" });
                }

                return Results.BadRequest(new { Message = "Échec de la réinitialisation du mot de passe" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la réinitialisation de mot de passe");
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("ResetPassword")
        .WithSummary("Réinitialiser mot de passe")
        .WithDescription("Réinitialise le mot de passe avec un token de réinitialisation")
        .Produces(200)
        .Produces(400)
        .Produces(500);

        // GET /api/v1/auth/me
        authGroup.MapGet("/me", async (
            IAuthenticationService authService,
            ClaimsPrincipal user) =>
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var userInfo = await authService.GetUserByIdAsync(userId);

                if (userInfo == null)
                {
                    return Results.NotFound(new { Message = "Utilisateur non trouvé" });
                }

                var roles = await authService.GetUserRolesAsync(userId);

                return Results.Ok(new UserInfoResponse
                {
                    UserId = userInfo.Id,
                    Email = userInfo.Email ?? string.Empty,
                    FirstName = userInfo.FirstName ?? string.Empty,
                    LastName = userInfo.LastName ?? string.Empty,
                    PhoneNumber = userInfo.PhoneNumber ?? string.Empty,
                    Roles = roles.ToList()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des informations utilisateur : {ex.Message}");
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("GetCurrentUser")
        .WithSummary("Informations utilisateur actuel")
        .WithDescription("Récupère les informations de l'utilisateur connecté")
        .RequireAuthorization()
        .Produces<UserInfoResponse>(200)
        .Produces(401)
        .Produces(404)
        .Produces(500);

        // POST /api/v1/auth/change-password
        authGroup.MapPost("/change-password", async (
            ChangePasswordRequest request,
            IAuthenticationService authService,
            ClaimsPrincipal user,
            ILogger<Program> logger) =>
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var success = await authService.ChangePasswordAsync(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword);

                if (success)
                {
                    logger.LogInformation("Changement de mot de passe réussi pour: {UserId}", userId);
                    return Results.Ok(new { Message = "Mot de passe changé avec succès" });
                }

                return Results.BadRequest(new { Message = "Échec du changement de mot de passe" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors du changement de mot de passe");
                return Results.Problem("Erreur interne du serveur");
            }
        })
        .WithName("ChangePassword")
        .WithSummary("Changer mot de passe")
        .WithDescription("Change le mot de passe de l'utilisateur connecté")
        .RequireAuthorization()
        .Produces(200)
        .Produces(400)
        .Produces(401)
        .Produces(500);
    }
}

// DTOs pour les requêtes et réponses
public record LoginRequest(
    string Email,
    string Password,
    bool RememberMe = false
);

public record LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? User { get; set; }
    public string? SessionId { get; set; }
    public TimeSpan? SessionDuration { get; set; }
    public List<string> UserRoles { get; set; } = new();
}

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber = null,
    string? Role = null
);

public record RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

public record UserInfoResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
