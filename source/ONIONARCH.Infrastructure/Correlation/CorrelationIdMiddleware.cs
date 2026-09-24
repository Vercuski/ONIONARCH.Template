using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ONIONARCH.Infrastructure.Correlation;

/// <summary>
/// ASP.NET Core middleware that establishes a correlation ID for every HTTP request.
/// </summary>
/// <remarks>
/// Reuses the caller's <see cref="HeaderName"/> header when present, otherwise generates a new GUID.
/// The ID is stored in <see cref="CorrelationIdAccessor"/>, echoed back on the response header, and
/// pushed as a <c>CorrelationId</c> logging scope for the rest of the pipeline. Register it (via
/// <c>UseCorrelationIdMiddleware()</c>) before <c>UseExceptionHandler()</c> so exception responses
/// can include the ID.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    /// <summary>
    /// The HTTP header used to receive and return the correlation ID.
    /// </summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>
    /// Resolves the correlation ID for the current request and invokes the rest of the pipeline
    /// inside a logging scope that carries it.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="accessor">The ambient correlation ID store (method-injected per request).</param>
    /// <param name="logger">The logger used to open the correlation ID scope.</param>
    /// <returns>A task that completes when the rest of the pipeline has finished.</returns>
    public async Task InvokeAsync(
        HttpContext context,
        CorrelationIdAccessor accessor,
        ILogger<CorrelationIdMiddleware> logger)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var incoming)
            && !string.IsNullOrWhiteSpace(incoming)
                ? incoming.ToString()
                : Guid.NewGuid().ToString();

        accessor.Set(correlationId);

        // Set before next() runs — response headers become read-only once the body starts writing.
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            await next(context);
        }
    }
}
