using ONIONARCH.Application.Exceptions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ONIONARCH.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptionsRegistration(builder.Configuration);
        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        return builder;
    }

    public static IServiceCollection AddOptionsRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionStringOptions>(GetSection<ConnectionStringOptions>(configuration));
        return services;
    }

    public static IConfigurationSection GetSection<T>(IConfiguration configuration)
        where T : BaseConfig
    {
        var config = Activator.CreateInstance(typeof(T))!;
        var section = ((BaseConfig)config).Section;
        return configuration.GetSection(section);
    }
}
