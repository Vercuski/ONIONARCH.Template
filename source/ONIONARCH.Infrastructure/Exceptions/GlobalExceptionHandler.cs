using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.Correlation;

namespace ONIONARCH.Infrastructure.Exceptions;

/// <summary>
/// Centralized handler for unhandled exceptions in every web host. Logs the exception and
/// returns an RFC 7807 <see cref="ProblemDetails"/> 500 response that includes the request's
/// correlation ID, without leaking exception details to the caller.
/// </summary>
/// <remarks>
/// Register with <c>AddExceptionHandler&lt;GlobalExceptionHandler&gt;()</c> and enable with
/// <c>UseExceptionHandler()</c>.
/// </remarks>
/// <param name="logger">The logger used to record the exception.</param>
/// <param name="correlationIdAccessor">Supplies the current request's correlation ID.</param>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    CorrelationIdAccessor correlationIdAccessor) : IExceptionHandler
{
    /// <summary>
    /// The logger used to record handled exceptions.
    /// </summary>
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    /// <summary>
    /// Supplies the correlation ID stamped onto the problem details response.
    /// </summary>
    private readonly CorrelationIdAccessor _correlationIdAccessor = correlationIdAccessor;

    /// <summary>
    /// Logs <paramref name="exception"/> and writes a 500 problem details response carrying a
    /// <c>correlationId</c> extension member.
    /// </summary>
    /// <param name="httpContext">The HTTP context of the failed request.</param>
    /// <param name="exception">The unhandled exception.</param>
    /// <param name="cancellationToken">A token to cancel writing the response.</param>
    /// <returns>Always <see langword="true"/>, indicating the exception has been handled.</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };
        problemDetails.Extensions["correlationId"] = _correlationIdAccessor.CorrelationId;

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
