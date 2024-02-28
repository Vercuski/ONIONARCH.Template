using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ONIONARCH.Infrastructure.HealthChecks;

namespace ONIONARCH.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecksRegistration();
        builder.Services.AddLoggingRegistration(builder.Environment);
        return builder;
    }

    public static IServiceCollection AddHealthChecksRegistration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<SimpleHealthCheck>("SimpleHealthCheck");
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
