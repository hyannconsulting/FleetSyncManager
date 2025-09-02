using System.ComponentModel.DataAnnotations;

namespace Laroche.FleetManager.Web.Models.Auth;

/// <summary>
/// Modèle pour la connexion utilisateur
/// </summary>
public class LoginModel
{
    [Required(ErrorMessage = "L'adresse e-mail est obligatoire")]
    [EmailAddress(ErrorMessage = "Format d'adresse e-mail invalide")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Le mot de passe est obligatoire")]
    public string Password { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; }
}

/// <summary>
/// Modèle pour l'inscription utilisateur
/// </summary>
public class RegisterModel
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Modèle pour la réinitialisation du mot de passe
/// </summary>
public class ForgotPasswordModel
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Modèle pour la réinitialisation avec token
/// </summary>
public class ResetPasswordModel
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Modèle pour le changement de mot de passe
/// </summary>
public class ChangePasswordModel
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Résultat d'une tentative de connexion
/// </summary>
public class LoginResult
{
    public bool IsSuccess { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Résultat d'une tentative d'inscription
/// </summary>
public class RegisterResult
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}

/// <summary>
/// Informations de l'utilisateur connecté
/// </summary>
public class UserInfoResult
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}
