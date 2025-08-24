using System.Net;
using System.Text.Json;

namespace Laroche.FleetManager.API.Middleware;

/// <summary>
/// Middleware pour la gestion centralisée des exceptions dans l'API
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Une erreur non gérée s'est produite dans l'API");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = "Une erreur interne s'est produite.",
            Detail = string.Empty,
            Instance = context.Request.Path,
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                response.Message = "Paramètre invalide.";
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Bad Request";
                response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case UnauthorizedAccessException:
                response.Message = "Accès non autorisé.";
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                response.Message = "Ressource non trouvée.";
                response.Status = (int)HttpStatusCode.NotFound;
                response.Title = "Not Found";
                response.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

/// <summary>
/// Modèle de réponse d'erreur conforme au RFC 7807 (Problem Details)
/// </summary>
public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
