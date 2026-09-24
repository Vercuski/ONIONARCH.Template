using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.Correlation;

namespace ONIONARCH.Presentation.Console;

/// <summary>
/// Sample background worker for the console host. Once per second it starts a new unit of work
/// with its own correlation ID and logs a heartbeat inside a correlation ID logging scope.
/// </summary>
/// <param name="logger">The logger used for heartbeat entries.</param>
/// <param name="correlationIdAccessor">The ambient correlation ID store.</param>
public class Worker(ILogger<Worker> logger, CorrelationIdAccessor correlationIdAccessor) : BackgroundService
{
    /// <summary>
    /// Runs the worker loop until the host signals shutdown.
    /// </summary>
    /// <param name="stoppingToken">Signaled when the host is stopping.</param>
    /// <returns>A task that completes when the loop exits.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Console has no HTTP pipeline to generate one, so each unit of work gets its own ID
            // the same way CorrelationIdMiddleware does for a web request.
            var correlationId = Guid.NewGuid().ToString();
            correlationIdAccessor.Set(correlationId);

            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                }
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
