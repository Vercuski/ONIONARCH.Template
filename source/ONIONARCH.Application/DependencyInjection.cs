using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Exceptions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Domain.Options;

namespace ONIONARCH.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptionsRegistration(builder.Configuration);
        builder.Services.AddMassTransitRegistration();
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
        services.Configure<RabbitMQOptions>(GetSection<RabbitMQOptions>(configuration));
        return services;
    }

    public static IServiceCollection AddMassTransitRegistration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var rabbitMQOptions = serviceProvider.GetService<IOptions<RabbitMQOptions>>()!.Value;

        services.AddMassTransit(setup =>
        {
            setup.UsingRabbitMq((context, config) =>
            {
                config.Host(rabbitMQOptions.Host, rabbitMQOptions.VirtualHost, h =>
                {
                    h.Username(rabbitMQOptions.Username);
                    h.Password(rabbitMQOptions.Password);
                });

                config.ConfigureEndpoints(context);
            });
        });
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
