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

/// <summary>
/// Composition-root extensions that register the Persistence layer: configuration options,
/// the database provider for each side of the CQRS split, and both the Dapper and EF Core
/// implementations of the Application-layer persistence ports.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Persistence-layer services.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <exception cref="InvalidOperationException">The <c>DatabasePlatform</c> configuration section is missing or invalid.</exception>
    /// <exception cref="NotSupportedException">A configured database platform is not recognized.</exception>
    public static IHostApplicationBuilder AddPersistenceRegistrations(this IHostApplicationBuilder builder)
    {
        builder.AddOptionsRegistration();
        builder.AddDatabaseProviderRegistration();
        return builder;
    }

    /// <summary>
    /// Binds <see cref="ConnectionStringOptions"/> and <see cref="DatabasePlatformOptions"/> to
    /// their configuration sections.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    private static IHostApplicationBuilder AddOptionsRegistration(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStringOptions>(GetSection<ConnectionStringOptions>(builder.Configuration));
        builder.Services.Configure<DatabasePlatformOptions>(GetSection<DatabasePlatformOptions>(builder.Configuration));
        return builder;
    }

    /// <summary>
    /// Reads <see cref="DatabasePlatformOptions"/> eagerly at startup, creates the query- and
    /// command-side <see cref="IDatabaseProvider"/>s, and registers both persistence paths with them.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <exception cref="InvalidOperationException">The <c>DatabasePlatform</c> configuration section is missing or invalid.</exception>
    private static IHostApplicationBuilder AddDatabaseProviderRegistration(
        this IHostApplicationBuilder builder)
    {
        var databasePlatformOptions = GetSection<DatabasePlatformOptions>(builder.Configuration)
            .Get<DatabasePlatformOptions>()
            ?? throw new InvalidOperationException("Missing or invalid 'DatabasePlatform' configuration section.");

        var queryDatabaseProvider = CreateDatabaseProvider(databasePlatformOptions.QueryDbPlatform, "Query");
        var commandDatabaseProvider = CreateDatabaseProvider(databasePlatformOptions.CommandDbPlatform, "Command");

        builder.AddDapperPersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);
        builder.AddEFCorePersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);

        return builder;
    }

    /// <summary>
    /// Maps a configured platform name to its <see cref="IDatabaseProvider"/>.
    /// </summary>
    /// <param name="platform">
    /// The platform name from configuration; case-insensitive. Supported values:
    /// <c>MSSQL</c>, <c>POSTGRESQL</c>, <c>MYSQL</c>.
    /// </param>
    /// <param name="side">"Query" or "Command"; used only in the error message.</param>
    /// <returns>The provider for <paramref name="platform"/>.</returns>
    /// <exception cref="NotSupportedException"><paramref name="platform"/> is not a supported value.</exception>
    private static IDatabaseProvider CreateDatabaseProvider(string platform, string side)
    {
        return platform.ToUpperInvariant() switch
        {
            "MSSQL" => new SqlServerDatabaseProvider(),
            "POSTGRESQL" => new PostgreSqlDatabaseProvider(),
            "MYSQL" => new MySQLDatabaseProvider(),
            _ => throw new NotSupportedException($"{side} Database platform '{platform}' is not supported.")
        };
    }

    /// <summary>
    /// Registers the Dapper path: read and write connection factories bound to their respective
    /// providers, plus the scoped Dapper query and command repositories.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <param name="queryDatabaseProvider">The provider for the query database.</param>
    /// <param name="commandDatabaseProvider">The provider for the command database.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
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

    /// <summary>
    /// Registers the EF Core path: <see cref="CommandDbContext"/> and <see cref="QueryDbContext"/>
    /// configured for their providers (with detailed errors and sensitive data logging outside
    /// Production), and maps <see cref="ICommandDbContext"/>, <see cref="IUnitOfWork"/>, and
    /// <see cref="IQueryDbContext"/> onto those scoped context instances.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <param name="queryDatabaseProvider">The provider for the query database.</param>
    /// <param name="commandDatabaseProvider">The provider for the command database.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
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
        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CommandDbContext>());

        return builder;
    }

    /// <summary>
    /// Returns the configuration section named by <typeparamref name="T"/>'s
    /// <see cref="IBaseOptionsConfig.Section"/>.
    /// </summary>
    /// <typeparam name="T">An options type with a public parameterless constructor.</typeparam>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The matching configuration section (empty if it does not exist).</returns>
    private static IConfigurationSection GetSection<T>(IConfiguration configuration)
    where T : IBaseOptionsConfig
    {
        var config = Activator.CreateInstance<T>()!;
        var section = ((IBaseOptionsConfig)config).Section;
        return configuration.GetSection(section);
    }
}