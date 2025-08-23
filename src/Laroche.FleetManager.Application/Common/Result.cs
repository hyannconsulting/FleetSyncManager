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
    /// Error message if operation failed
    /// </summary>
    public string? ErrorMessage { get; private init; }
    
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

/// <summary>
/// Paged result wrapper
/// </summary>
/// <typeparam name="T">Item type</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// List of items for current page
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = [];
    
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
    
    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    public bool HasPreviousPage => Page > 1;
    
    /// <summary>
    /// Creates a paged result
    /// </summary>
    /// <param name="items">List of items</param>
    /// <param name="totalCount">Total count</param>
    /// <param name="page">Current page</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paged result</returns>
    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
