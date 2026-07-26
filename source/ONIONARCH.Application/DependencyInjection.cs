using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationRegistration(this IHostApplicationBuilder builder)
    {
        builder.AddMediatorRegistration();
        return builder;
    }

    private static IHostApplicationBuilder AddMediatorRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });
        return builder;
    }
}
