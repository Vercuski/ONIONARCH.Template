// ONIONARCH.Persistence/DependencyInjection.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Persistence.ConnectionFactory;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;

namespace ONIONARCH.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistenceRegistrations(this IHostApplicationBuilder builder, IDatabaseProvider databaseProvider)
    {
        builder.AddOptionsRegistration();
        builder.AddDapperPersistenceRegistrations(databaseProvider);
        builder.AddEFCorePersistenceRegistrations(databaseProvider);
        return builder;
    }

    private static IHostApplicationBuilder AddOptionsRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStringOptions>(GetSection<ConnectionStringOptions>(builder.Configuration));
        return builder;
    }

    private static IHostApplicationBuilder AddDapperPersistenceRegistrations(
        this IHostApplicationBuilder builder,
        IDatabaseProvider databaseProvider)
    {
        builder.Services.AddScoped<IDbReadOnlyConnectionFactory>(sp =>
            new DbReadOnlyConnectionFactory(
                sp.GetRequiredService<IOptions<ConnectionStringOptions>>(),
                databaseProvider));

        builder.Services.AddScoped<IDbWriteConnectionFactory>(sp =>
            new DbWriteConnectionFactory(
                sp.GetRequiredService<IOptions<ConnectionStringOptions>>(),
                databaseProvider));

        return builder;
    }

    private static IHostApplicationBuilder AddEFCorePersistenceRegistrations(
        this IHostApplicationBuilder builder,
        IDatabaseProvider databaseProvider)
    {
        builder.Services.AddDbContext<CommandDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            databaseProvider.ConfigureEfCore(options, connectionStringOptions.CommandDbConnection);
            if (!builder.Environment.IsProduction())
            {
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
            }
        }, ServiceLifetime.Scoped);

        builder.Services.AddDbContext<QueryDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            databaseProvider.ConfigureEfCore(options, connectionStringOptions.QueryDbConnection);
            if (!builder.Environment.IsProduction())
            {
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
            }
        }, ServiceLifetime.Scoped);

        builder.Services.AddScoped<ICommandDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        builder.Services.AddScoped<IQueryDbContext>(sp => sp.GetRequiredService<QueryDbContext>());
        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CommandDbContext>());

        return builder;
    }

    private static IConfigurationSection GetSection<T>(IConfiguration configuration)
    where T : IBaseOptionsConfig
    {
        var config = Activator.CreateInstance<T>()!;
        var section = ((IBaseOptionsConfig)config).Section;
        return configuration.GetSection(section);
    }
}