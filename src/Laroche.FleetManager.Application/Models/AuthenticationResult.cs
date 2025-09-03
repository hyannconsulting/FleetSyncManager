using Laroche.FleetManager.Domain.Enums;

namespace Laroche.FleetManager.Application.Models;

/// <summary>
/// Résultat d'une tentative d'authentification
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }
    public LoginResult Result { get; set; }
    public string Message { get; set; } = string.Empty;
    //public UserDto? User { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public TimeSpan? SessionDuration { get; set; }
    //public string? UserId { get; set; }

    ///// <summary>
    ///// Crée un résultat de succès
    ///// </summary>
    //public static AuthenticationResult Success(UserDto? user,
    //    string sessionId,
    //    List<string> roles,
    //    TimeSpan sessionDuration)
    //{
    //    return new AuthenticationResult
    //    {
    //        IsSuccess = true,
    //        Result = LoginResult.Success,
    //        Message = "Connexion réussie",
    //        User = user,
    //        SessionId = sessionId,
    //        SessionDuration = sessionDuration
    //    };
    //}

    /// <summary>
    /// Creates a successful authentication result with the specified token, session ID, and session duration.
    /// </summary>
    /// <param name="token">The authentication token issued to the user.</param>
    /// <param name="sessionId">The unique identifier for the user's session.</param>
    /// <param name="sessionDuration">The duration for which the session is valid.</param>
    /// <returns>An <see cref="AuthenticationResult"/> representing a successful authentication.</returns>
    public static AuthenticationResult Success(
       string token,
       string sessionId,
       TimeSpan sessionDuration)
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            Result = LoginResult.Success,
            Message = "Connexion réussie",
            Token = token,
            SessionId = sessionId,
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
