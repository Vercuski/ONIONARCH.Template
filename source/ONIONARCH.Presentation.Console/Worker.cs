using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.Correlation;

namespace ONIONARCH.Presentation.Console;

public class Worker(ILogger<Worker> logger, CorrelationIdAccessor correlationIdAccessor) : BackgroundService
{
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
