// ONIONARCH.Persistence/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Options;
using ONIONARCH.Persistence.ConnectionFactory;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Persistence.Providers;

namespace ONIONARCH.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddDapperPersistenceRegistrations(
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

    public static IHostApplicationBuilder AddEFCorePersistenceRegistrations(
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
}