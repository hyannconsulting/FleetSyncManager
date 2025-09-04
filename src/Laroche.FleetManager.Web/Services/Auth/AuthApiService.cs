using Laroche.FleetManager.Web.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Laroche.FleetManager.Web.Services.Auth;

/// <summary>
/// Implémentation du service d'authentification via API HTTP
/// </summary>
public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var loginRequest = new
            {
                email,
                password
            };

            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginApiResponse>(responseContent, _jsonOptions);

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Stocker le token dans les cookies sécurisés
                    // await SetAuthenticationCookieAsync(loginResponse.Token, loginResponse.ExpiresAt);

                    return new LoginResult
                    {
                        IsSuccess = true,
                        Token = loginResponse.Token,
                        ExpiresAt = loginResponse.ExpiresAt,
                        SessionDuration = loginResponse.SessionDuration,
                        SessionID = loginResponse.SessionId,
                        RefreshToken = loginResponse.RefreshToken
                    };
                }
            }

            var errorResponse = JsonSerializer.Deserialize<ErrorApiResponse>(responseContent, _jsonOptions);
            return new LoginResult
            {
                IsSuccess = false,
                Errors = errorResponse?.Errors ?? new List<string> { "Erreur de connexion" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion pour {Email}", email);
            return new LoginResult
            {
                IsSuccess = false,
                Errors = new List<string> { "Erreur interne lors de la connexion" }
            };
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            var token = await GetCurrentTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await _httpClient.PostAsync("/api/v1/auth/logout", null);
            }

            // Supprimer les cookies d'authentification
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la déconnexion");
            return false;
        }
    }

    public async Task<RegisterResult> RegisterAsync(RegisterModel model)
    {
        try
        {
            var registerRequest = new
            {
                email = model.Email,
                password = model.Password,
                confirmPassword = model.ConfirmPassword,
                firstName = model.FirstName,
                lastName = model.LastName,
                phoneNumber = model.PhoneNumber
            };

            var json = JsonSerializer.Serialize(registerRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/auth/register", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return new RegisterResult { IsSuccess = true };
            }

            var errorResponse = JsonSerializer.Deserialize<ErrorApiResponse>(responseContent, _jsonOptions);
            return new RegisterResult
            {
                IsSuccess = false,
                Errors = errorResponse?.Errors ?? new List<string> { "Erreur lors de l'inscription" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription pour {Email}", model.Email);
            return new RegisterResult
            {
                IsSuccess = false,
                Errors = new List<string> { "Erreur interne lors de l'inscription" }
            };
        }
    }

    public async Task<UserInfoResult?> GetCurrentUserAsync()
    {
        try
        {
            var token = await GetCurrentTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("/api/v1/auth/me");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfoApiResponse>(responseContent, _jsonOptions);

                if (userInfo != null)
                {
                    return new UserInfoResult
                    {
                        UserId = userInfo.UserId,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        PhoneNumber = userInfo.PhoneNumber,
                        Roles = userInfo.Roles
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des informations utilisateur");
            return null;
        }
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var request = new { email };
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/auth/forgot-password", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la demande de réinitialisation du mot de passe pour {Email}", email);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            var request = new
            {
                email = email,
                token = token,
                newPassword = newPassword
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/v1/auth/reset-password", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe pour {Email}", email);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            var token = await GetCurrentTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var request = new
            {
                currentPassword,
                newPassword
            };

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync("/api/v1/auth/change-password", content);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du changement de mot de passe");
            return false;
        }
    }

    private async Task<string?> GetCurrentTokenAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                return result.Properties.GetTokenValue("access_token");
            }
        }
        return null;
    }

    private async Task SetAuthenticationCookieAsync(string token, DateTime? expiresAt)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var properties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = expiresAt?.ToUniversalTime()
        };

        properties.StoreTokens(new[]
        {
            new AuthenticationToken
            {
                Name = "access_token",
                Value = token
            }
        });

        // Pour simplifier, nous utilisons une identité basique
        // En production, vous pourriez vouloir décoder le JWT pour extraire les claims
        var identity = new System.Security.Claims.ClaimsIdentity(
            new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "User"),
                new System.Security.Claims.Claim("access_token", token)
            },
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }

    // Classes internes pour désérialiser les réponses API
    private class LoginApiResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? SessionId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public TimeSpan? SessionDuration { get; set; }
    }

    private class UserInfoApiResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }

    private class ErrorApiResponse
    {
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }
}
