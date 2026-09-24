using Microsoft.Extensions.Logging;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Application.Behaviors;

/// <summary>
/// Logs entry, successful completion, and failure of every command/query. Combined with
/// the ambient correlation-ID logging scope pushed by CorrelationIdMiddleware (Infrastructure),
/// this reconstructs the full "path" of a request through the CQRS pipeline without any handler
/// needing to know a correlation ID exists.
/// </summary>
/// <typeparam name="TRequest">The request type being dispatched.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request's handler.</typeparam>
/// <param name="logger">The logger used to write pipeline entries.</param>
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    /// <summary>
    /// Logs the request name before and after invoking <paramref name="next"/>. Exceptions are
    /// logged at error level and rethrown unchanged so the global exception handler still sees them.
    /// </summary>
    /// <param name="request">The request being dispatched.</param>
    /// <param name="next">Invokes the rest of the pipeline.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The response produced by <paramref name="next"/>.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next();
            logger.LogInformation("Handled {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{RequestName} failed", requestName);
            throw;
        }
    }
}
