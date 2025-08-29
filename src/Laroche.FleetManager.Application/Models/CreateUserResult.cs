using Laroche.FleetManager.Domain.Entities;

namespace Laroche.FleetManager.Application.Models;

/// <summary>
/// Résultat de la création d'un utilisateur
/// </summary>
public class CreateUserResult
{
    /// <summary>
    /// Indique si la création a réussi
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Message de résultat
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Liste des erreurs de validation
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Utilisateur créé (si succès)
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// ID de l'utilisateur créé
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Crée un résultat de succès
    /// </summary>
    public static CreateUserResult Success(string userId, string message)
    {
        return new CreateUserResult
        {
            IsSuccess = true,
            Message = message,
            UserId = userId
        };
    }

    /// <summary>
    /// Crée un résultat d'échec
    /// </summary>
    public static CreateUserResult Failure(string message, List<string>? errors = null)
    {
        return new CreateUserResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
