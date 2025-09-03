using Laroche.FleetManager.Application.DTOs.Users;
using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.Models;

/// <summary>
/// Résultat d'une tentative d'authentification
/// </summary>
public class AuthenticationResult
{
    public bool IsSuccess { get; set; }
    public LoginResult Result { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string? SessionId { get; set; }
    public TimeSpan? SessionDuration { get; set; }
    public List<string> UserRoles { get; set; } = new();
    //public string? UserId { get; set; }

    /// <summary>
    /// Crée un résultat de succès
    /// </summary>
    public static AuthenticationResult Success(UserDto? user,
        string sessionId,
        List<string> roles,
        TimeSpan sessionDuration)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            Result = LoginResult.Success,
            Message = "Connexion réussie",
            User = user,
            SessionId = sessionId,
            UserRoles = roles,
            SessionDuration = sessionDuration
        };
    }

    /// <summary>
    /// Crée un résultat d'échec
    /// </summary>
    public static AuthenticationResult Failure(string message, LoginResult result = LoginResult.InvalidCredentials)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            Result = result,
            Message = message
        };
    }
}
