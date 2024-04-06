using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.HealthChecks;
using System.Reflection;

namespace ONIONARCH.Infrastructure;

public static class DependencyInjection
{
    public static WebApplication? AddInfrastructureApplicationRegistration(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckConfiguration.WriteResponse
        });
        return app;
    }

    public static IHostApplicationBuilder AddInfrastructureRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecksRegistration();
        builder.Services.AddLoggingRegistration(builder.Environment);
        return builder;
    }

    public static IServiceCollection AddHealthChecksRegistration(this IServiceCollection services)
    {
        var healthCheckBuilder = services.AddHealthChecks();
        foreach (var healthCheckType in Assembly.GetExecutingAssembly()
            .GetTypes().Where(type => !type.IsAbstract &&
            type.GetInterfaces().Contains(typeof(IHealthCheck))))
        {
            healthCheckBuilder.AddCheck(healthCheckType.Name,
                (IHealthCheck)Activator.CreateInstance(healthCheckType)!);
        }
        return services;
    }

    public static IServiceCollection AddLoggingRegistration(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddLogging(config =>
        {
            config.ClearProviders();
            if (!environment.IsProduction())
            {
                config.AddConsole();
            }
        });
        return services;
    }
}
