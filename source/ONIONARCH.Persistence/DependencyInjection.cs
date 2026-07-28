// ONIONARCH.Persistence/DependencyInjection.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Application.Abstractions.Repositories;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Persistence.ConnectionFactory;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;
using ONIONARCH.Persistence.Repositories;

namespace ONIONARCH.Persistence;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddPersistenceRegistrations(this IHostApplicationBuilder builder)
    {
        builder.AddOptionsRegistration();
        builder.AddDatabaseProviderRegistration();
        return builder;
    }
    
    private static IHostApplicationBuilder AddOptionsRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStringOptions>(GetSection<ConnectionStringOptions>(builder.Configuration));
        builder.Services.Configure<DatabasePlatformOptions>(GetSection<DatabasePlatformOptions >(builder.Configuration));
        return builder;
    }

    private static IHostApplicationBuilder AddDatabaseProviderRegistration(
        this IHostApplicationBuilder builder)
    {
        var databasePlatformOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<DatabasePlatformOptions>>().Value;
        
        IDatabaseProvider queryDatabaseProvider = databasePlatformOptions.QueryDbPlatform.ToUpper()
        switch
        {
            "MSSQL" => new SqlServerDatabaseProvider(),
            "POSTGRESQL" => new PostgreSqlDatabaseProvider(),
            "MYSQL" => new MySQLDatabaseProvider(),
            _ => throw new NotSupportedException($"Query Database platform '{databasePlatformOptions.QueryDbPlatform}' is not supported.")
        };

        IDatabaseProvider commandDatabaseProvider = databasePlatformOptions.CommandDbPlatform.ToUpper()
        switch
        {
            "MSSQL" => new SqlServerDatabaseProvider(),
            "POSTGRESQL" => new PostgreSqlDatabaseProvider(),
            "MYSQL" => new MySQLDatabaseProvider(),
            _ => throw new NotSupportedException($"Command Database platform '{databasePlatformOptions.CommandDbPlatform}' is not supported.")
        };

        builder.AddDapperPersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);
        builder.AddEFCorePersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);

        return builder;
    }

    private static IHostApplicationBuilder AddDapperPersistenceRegistrations(
        this IHostApplicationBuilder builder,
        IDatabaseProvider queryDatabaseProvider,
        IDatabaseProvider commandDatabaseProvider)
    {
        builder.Services.AddScoped<IDbReadOnlyConnectionFactory>(sp =>
            new DbReadOnlyConnectionFactory(
                sp.GetRequiredService<IOptions<ConnectionStringOptions>>(),
                queryDatabaseProvider));

        builder.Services.AddScoped<IDbWriteConnectionFactory>(sp =>
            new DbWriteConnectionFactory(
                sp.GetRequiredService<IOptions<ConnectionStringOptions>>(),
                commandDatabaseProvider));

        builder.Services.AddScoped<ISampleEntityDapperQueryRepository, SampleEntityDapperQueryRepository>();
        builder.Services.AddScoped<ISampleEntityDapperCommandRepository, SampleEntityDapperCommandRepository>();

        return builder;
    }

    private static IHostApplicationBuilder AddEFCorePersistenceRegistrations(
        this IHostApplicationBuilder builder,
        IDatabaseProvider queryDatabaseProvider,
        IDatabaseProvider commandDatabaseProvider)
    {
        builder.Services.AddDbContext<CommandDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            commandDatabaseProvider.ConfigureEfCore(options, connectionStringOptions.CommandDbConnection);
            if (!builder.Environment.IsProduction())
            {
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
            }
        }, ServiceLifetime.Scoped);

        builder.Services.AddDbContext<QueryDbContext>((sp, options) =>
        {
            var connectionStringOptions = sp.GetRequiredService<IOptions<ConnectionStringOptions>>().Value;
            queryDatabaseProvider.ConfigureEfCore(options, connectionStringOptions.QueryDbConnection);
            if (!builder.Environment.IsProduction())
            {
                options.EnableDetailedErrors().EnableSensitiveDataLogging();
            }
        }, ServiceLifetime.Scoped);

        builder.Services.AddScoped<ICommandDbContext>(sp => sp.GetRequiredService<CommandDbContext>());
        builder.Services.AddScoped<IQueryDbContext>(sp => sp.GetRequiredService<QueryDbContext>());

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