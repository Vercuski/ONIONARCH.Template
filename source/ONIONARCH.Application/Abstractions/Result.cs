namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Categorizes an expected business-flow failure carried by a <see cref="Result{T}"/>.
/// Presentation maps each category to an HTTP status code.
/// </summary>
public enum ResultErrorType
{
    /// <summary>The requested resource does not exist (maps to HTTP 404).</summary>
    NotFound,

    /// <summary>The request failed business validation (maps to HTTP 400).</summary>
    Validation,

    /// <summary>The request conflicts with the current state of the resource (maps to HTTP 409).</summary>
    Conflict
}

/// <summary>
/// Outcome of a request handler: either a successful value or an expected business-flow failure.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <remarks>
/// Use this only for failures that are part of normal business flow (not found, validation,
/// conflict). Genuine infrastructure exceptions should propagate so the global exception handler
/// can log them and return a 500 response, rather than being hidden inside a failed result.
/// </remarks>
public readonly struct Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the success value, or the default of <typeparamref name="T"/> when <see cref="IsSuccess"/> is <see langword="false"/>.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets a description of the failure, or <see langword="null"/> when <see cref="IsSuccess"/> is <see langword="true"/>.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the failure category. Only meaningful when <see cref="IsSuccess"/> is <see langword="false"/>;
    /// on success it holds the enum's default value.
    /// </summary>
    public ResultErrorType ErrorType { get; }

    /// <summary>
    /// Initializes a new result. Use <see cref="Success"/> or <see cref="Failure"/> instead.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="value">The success value.</param>
    /// <param name="error">The failure description.</param>
    /// <param name="errorType">The failure category.</param>
    private Result(bool isSuccess, T? value, string? error, ResultErrorType errorType)
    {
        (IsSuccess, Value, Error, ErrorType) = (isSuccess, value, error, errorType);
    }

    /// <summary>
    /// Creates a successful result carrying <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A result whose <see cref="IsSuccess"/> is <see langword="true"/>.</returns>
    public static Result<T> Success(T value)
    {
        return new(true, value, null, default);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">A description of the failure, suitable for returning to the caller.</param>
    /// <param name="errorType">The failure category.</param>
    /// <returns>A result whose <see cref="IsSuccess"/> is <see langword="false"/>.</returns>
    public static Result<T> Failure(string error, ResultErrorType errorType)
    {
        return new(false, default, error, errorType);
    }
}