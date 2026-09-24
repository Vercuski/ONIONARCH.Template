using Microsoft.AspNetCore.Mvc;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Presentation.API.Extensions;

/// <summary>
/// Maps Application-layer <see cref="Result{T}"/> values to MVC <see cref="IActionResult"/>s so
/// controllers share one consistent status-code mapping.
/// </summary>
/// <remarks>
/// Failure mapping: <see cref="ResultErrorType.NotFound"/> → 404,
/// <see cref="ResultErrorType.Validation"/> → 400, <see cref="ResultErrorType.Conflict"/> → 409,
/// anything else → 500 problem details.
/// </remarks>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a <see cref="Result{T}"/> directly to an <see cref="IActionResult"/>.
    /// On success, returns 200 OK with <c>result.Value</c> as-is.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The handler result to map.</param>
    /// <param name="controller">The controller used to create the response.</param>
    /// <returns>200 OK on success; otherwise the error response for <see cref="Result{T}.ErrorType"/>.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.IsSuccess
            ? controller.Ok(result.Value)
            : result.ToErrorResult(controller);
    }

    /// <summary>
    /// Maps a <see cref="Result{T}"/> to an <see cref="IActionResult"/>, projecting the
    /// success value through <paramref name="onSuccess"/> (e.g. entity -&gt; DTO) before
    /// returning it via 200 OK. Mirrors the original controller behavior of treating a
    /// success with a null value as not-found.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TResponse">The type returned to the caller.</typeparam>
    /// <param name="result">The handler result to map.</param>
    /// <param name="controller">The controller used to create the response.</param>
    /// <param name="onSuccess">Projects the success value into the response body.</param>
    /// <returns>200 OK with the projected value on success; otherwise the mapped error response.</returns>
    /// <remarks>
    /// A success with a <see langword="null"/> value falls through to the error mapping; because
    /// <see cref="Result{T}.ErrorType"/> defaults to <see cref="ResultErrorType.NotFound"/>, that yields 404.
    /// </remarks>
    public static IActionResult ToActionResult<T, TResponse>(
        this Result<T> result,
        ControllerBase controller,
        Func<T, TResponse> onSuccess)
    {
        return result.IsSuccess && result.Value is not null
            ? controller.Ok(onSuccess(result.Value))
            : result.ToErrorResult(controller);
    }

    /// <summary>
    /// Maps a failed result's <see cref="Result{T}.ErrorType"/> to the matching error response,
    /// using <see cref="Result{T}.Error"/> as the body.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <param name="result">The failed result to map.</param>
    /// <param name="controller">The controller used to create the response.</param>
    /// <returns>A 404, 400, 409, or 500 object result.</returns>
    private static ObjectResult ToErrorResult<T>(this Result<T> result, ControllerBase controller)
    {
        return result.ErrorType switch
        {
            ResultErrorType.NotFound => controller.NotFound(result.Error),
            ResultErrorType.Validation => controller.BadRequest(result.Error),
            ResultErrorType.Conflict => controller.Conflict(result.Error),
            _ => controller.Problem(result.Error)
        };
    }
}
