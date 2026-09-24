using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.Correlation;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Infrastructure.Versioning;
using System.Reflection;

namespace ONIONARCH.Infrastructure;

/// <summary>
/// Composition-root extensions that register and wire up the Infrastructure layer's
/// cross-cutting services (health checks, logging, correlation IDs, problem details).
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Maps the <c>/health</c> endpoint, formatting the report with
    /// <see cref="HealthCheckConfiguration.WriteResponse"/>.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The same <paramref name="app"/>, for chaining.</returns>
    public static WebApplication? AddInfrastructureApplicationRegistration(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckConfiguration.WriteResponse
        });
        return app;
    }

    /// <summary>
    /// Adds <see cref="CorrelationIdMiddleware"/> to the request pipeline.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The same <paramref name="app"/>, for chaining.</returns>
    /// <remarks>
    /// Call this before <c>UseExceptionHandler()</c> so the correlation ID is already set when
    /// the global exception handler builds its response.
    /// </remarks>
    public static WebApplication UseCorrelationIdMiddleware(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        return app;
    }

    /// <summary>
    /// Registers the Infrastructure layer's services: health checks, logging providers,
    /// the singleton <see cref="CorrelationIdAccessor"/>, the singleton <see cref="ApplicationVersion"/>
    /// (plus a hosted service that logs it at startup), and ProblemDetails support.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <remarks>
    /// <c>AddProblemDetails()</c> is required when a host pairs <c>AddExceptionHandler&lt;T&gt;()</c>
    /// with the parameterless <c>UseExceptionHandler()</c>; without it the host throws at startup.
    /// </remarks>
    public static IHostApplicationBuilder AddInfrastructureRegistration(this IHostApplicationBuilder builder)
    {
        builder.AddHealthChecksRegistration();
        builder.AddLoggingRegistration();
        builder.Services.AddSingleton<CorrelationIdAccessor>();
        builder.Services.AddSingleton(ApplicationVersion.Current);
        builder.Services.AddHostedService<ApplicationVersionLogger>();
        builder.Services.AddProblemDetails();
        return builder;
    }

    /// <summary>
    /// Registers every concrete <see cref="IHealthCheck"/> implementation found in this assembly,
    /// named after its type and created through <see cref="ActivatorUtilities"/> so it can take
    /// constructor dependencies.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    private static IHostApplicationBuilder AddHealthChecksRegistration(this IHostApplicationBuilder builder)
    {
        var healthCheckBuilder = builder.Services.AddHealthChecks();
        foreach (var healthCheckType in Assembly.GetExecutingAssembly()
            .GetTypes().Where(type => !type.IsAbstract &&
            type.GetInterfaces().Contains(typeof(IHealthCheck))))
        {
            healthCheckBuilder.Add(new HealthCheckRegistration(
                healthCheckType.Name,
                serviceProvider => (IHealthCheck)ActivatorUtilities.CreateInstance(serviceProvider, healthCheckType),
                failureStatus: null,
                tags: null));
        }
        return builder;
    }

    /// <summary>
    /// Replaces the default logging providers. Outside Production, a simple console logger with
    /// scopes enabled is added so correlation IDs appear on each line; in Production no provider
    /// is added here, so one must be configured elsewhere for logs to be emitted.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    private static IHostApplicationBuilder AddLoggingRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddLogging(config =>
        {
            config.ClearProviders();
            if (!builder.Environment.IsProduction())
            {
                config.AddSimpleConsole(options => options.IncludeScopes = true);
            }
        });
        return builder;
    }
}
