using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Laroche.FleetManager.Application.Common;

namespace Laroche.FleetManager.Application.Behaviors;

/// <summary>
/// Pipeline behavior for request validation using FluentValidation
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes validation behavior
    /// </summary>
    /// <param name="validators">List of validators for the request</param>
    /// <param name="logger">Logger instance</param>
    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    /// <summary>
    /// Handles validation pipeline
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">Next handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
        {
            _logger.LogWarning("Validation failed for {RequestType}. Errors: {Errors}", 
                typeof(TRequest).Name, 
                string.Join(", ", failures.Select(f => f.ErrorMessage)));
                
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();
            
            // If TResponse is Result<T>, return validation failure
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var validationFailureMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("ValidationFailure");
                var result = validationFailureMethod?.Invoke(null, new object[] { errorMessages });
                return (TResponse)result!;
            }
            
            // If TResponse is Result, return validation failure
            if (typeof(TResponse) == typeof(Result))
            {
                var result = Result.ValidationFailure(errorMessages);
                return (TResponse)(object)result;
            }
            
            // For other response types, throw validation exception
            throw new ValidationException(failures);
        }

        return await next();
    }
}

/// <summary>
/// Pipeline behavior for logging requests and responses
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes logging behavior
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles logging pipeline
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="next">Next handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid();
        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        _logger.LogInformation("Starting request {RequestName}", requestNameWithGuid);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            stopwatch.Stop();
            
            _logger.LogInformation("Completed request {RequestName} in {ElapsedMilliseconds}ms", 
                requestNameWithGuid, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Request {RequestName} failed after {ElapsedMilliseconds}ms", 
                requestNameWithGuid, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
