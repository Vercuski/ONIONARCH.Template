using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
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
    public static IHostApplicationBuilder AddApplicationRegistration(this WebApplicationBuilder builder)
    {
        builder.AddOptionsRegistration();
        builder.AddMassTransitRegistration();
        builder.AddMediatorRegistration();
        builder.AddErrorHandling();
        return builder;
    }

    private static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        return builder;
    }

    private static WebApplicationBuilder AddMediatorRegistration(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });
        return builder;
    }

    private static WebApplicationBuilder AddOptionsRegistration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStringOptions>(GetSection<ConnectionStringOptions>(builder.Configuration));
        builder.Services.Configure<RabbitMQOptions>(GetSection<RabbitMQOptions>(builder.Configuration));
        builder.Services.Configure<LogOptions>(GetSection<LogOptions>(builder.Configuration));
        return builder;
    }

    private static WebApplicationBuilder AddMassTransitRegistration(this WebApplicationBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var rabbitMQOptions = serviceProvider.GetService<IOptions<RabbitMQOptions>>()!.Value;

        builder.Services.AddMassTransit(setup =>
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
        return builder;
    }

    private static IConfigurationSection GetSection<T>(IConfiguration configuration)
        where T : BaseOptionsConfig
    {
        var config = Activator.CreateInstance(typeof(T))!;
        var section = ((BaseOptionsConfig)config).Section;
        return configuration.GetSection(section);
    }
}
