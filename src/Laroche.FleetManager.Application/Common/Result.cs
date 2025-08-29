namespace Laroche.FleetManager.Application.Common;

/// <summary>
/// Generic response wrapper for API operations
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; private init; }
    
    /// <summary>
    /// Response data
    /// </summary>
    public T? Data { get; private init; }
    
    /// <summary>
    /// Response data (alias for compatibility)
    /// </summary>
    public T? Value => Data;
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; private init; }
    
    /// <summary>
    /// Error message (alias for compatibility)
    /// </summary>
    public string? Error => ErrorMessage;
    
    /// <summary>
    /// List of validation errors
    /// </summary>
    public IList<string> ValidationErrors { get; private init; } = [];
    
    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="data">Response data</param>
    /// <returns>Success result</returns>
    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failure result</returns>
    public static Result<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
    
    /// <summary>
    /// Creates a validation failure result
    /// </summary>
    /// <param name="validationErrors">List of validation errors</param>
    /// <returns>Validation failure result</returns>
    public static Result<T> ValidationFailure(IList<string> validationErrors) => new()
    {
        IsSuccess = false,
        ValidationErrors = validationErrors,
        ErrorMessage = "Validation failed"
    };
}

/// <summary>
/// Non-generic result for operations without return data
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool IsSuccess { get; private init; }
    
    /// <summary>
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; private init; }
    
    /// <summary>
    /// Error message (alias for compatibility)
    /// </summary>
    public string? Error => ErrorMessage;
    
    /// <summary>
    /// List of validation errors
    /// </summary>
    public IList<string> ValidationErrors { get; private init; } = [];
    
    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>Success result</returns>
    public static Result Success() => new()
    {
        IsSuccess = true
    };
    
    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    /// <returns>Failure result</returns>
    public static Result Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
    
    /// <summary>
    /// Creates a validation failure result
    /// </summary>
    /// <param name="validationErrors">List of validation errors</param>
    /// <returns>Validation failure result</returns>
    public static Result ValidationFailure(IList<string> validationErrors) => new()
    {
        IsSuccess = false,
        ValidationErrors = validationErrors,
        ErrorMessage = "Validation failed"
    };
}
