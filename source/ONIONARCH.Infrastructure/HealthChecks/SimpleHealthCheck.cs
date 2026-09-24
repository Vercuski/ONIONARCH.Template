using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace ONIONARCH.Infrastructure.HealthChecks;

/// <summary>
/// Demonstration health check that randomly reports Unhealthy, Degraded, or Healthy so the
/// <c>/health</c> endpoint's output for each status can be observed. Replace the code between the
/// Start/End markers with a real probe (database, downstream service, etc.).
/// </summary>
/// <remarks>
/// Discovered and registered automatically by the Infrastructure health check assembly scan.
/// </remarks>
public class SimpleHealthCheck : IHealthCheck
{
    /// <summary>
    /// Runs the health check.
    /// </summary>
    /// <param name="context">Context for the health check registration being evaluated.</param>
    /// <param name="cancellationToken">A token to cancel the check.</param>
    /// <returns>
    /// A result whose status is chosen at random, with the random value included in its data.
    /// </returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        HealthCheckResult result;
        /* Start Health Check Code */
        Random rand = new();
        var test = rand.Next(1, 4);

        IReadOnlyDictionary<string, object> data = new Dictionary<string, object>
            {
                { "Value", test }
            };

        if (test == 1)
        {
            Exception ex = new("Value 1 Exception");
            result = new HealthCheckResult(HealthStatus.Unhealthy, "Value was 1", ex, data);
        }
        else if (test == 2)
        {
            result = new HealthCheckResult(HealthStatus.Degraded, "Value was 2", null, data);
        }
        else
        {
            result = new HealthCheckResult(HealthStatus.Healthy, "Value was 3", null, data);
        }
        /* End Health Check Code */

        stopwatch.Stop();

        return Task.FromResult(result);
    }
}
