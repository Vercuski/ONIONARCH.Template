using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
//#if (HasEfCore)
using ONIONARCH.Application.Abstractions;
//#endif
//#if (HasDapper)
using ONIONARCH.Application.Abstractions.ConnectionFactory;
//#endif
//#if (HasEfCore)
using ONIONARCH.Application.Abstractions.Context;
//#endif
//#if (HasDapper && IncludeSamples)
using ONIONARCH.Application.Abstractions.Repositories;
//#endif
using ONIONARCH.Domain.Abstractions;
//#if (HasEfCore)
using ONIONARCH.Persistence.Bulk;
//#endif
//#if (HasDapper)
using ONIONARCH.Persistence.ConnectionFactory;
//#endif
//#if (HasEfCore)
using ONIONARCH.Persistence.Contexts;
//#endif
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;
//#if (HasDapper && IncludeSamples)
using ONIONARCH.Persistence.Repositories;
//#endif

namespace ONIONARCH.Persistence;

/// <summary>
/// Composition-root extensions that register the Persistence layer: configuration options,
/// the database provider for each side of the CQRS split (resolved from the providers the host
/// registers in a <see cref="DatabaseProviderRegistry"/>), and both the Dapper and EF Core
/// implementations of the Application-layer persistence ports.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Persistence-layer services.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <param name="configureProviders">
    /// Opts the host in to the provider-specific projects it references, e.g.
    /// <c>providers.AddSqlServer()</c>. The query and command sides may use different platforms, so
    /// register every platform either side's configuration can name.
    /// </param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configureProviders"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">The <c>DatabasePlatform</c> configuration section is missing or invalid.</exception>
    /// <exception cref="NotSupportedException">A configured database platform has no registered provider.</exception>
    public static IHostApplicationBuilder AddPersistenceRegistrations(
        this IHostApplicationBuilder builder,
        Action<DatabaseProviderRegistry> configureProviders)
    {
        ArgumentNullException.ThrowIfNull(configureProviders);

        var registry = new DatabaseProviderRegistry();
        configureProviders(registry);

        builder.AddOptionsRegistration();
        builder.AddDatabaseProviderRegistration(registry);
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
    /// Reads <see cref="DatabasePlatformOptions"/> eagerly at startup, resolves the query- and
    /// command-side <see cref="IDatabaseProvider"/>s from <paramref name="registry"/>, and registers
    /// both persistence paths with them.
    /// </summary>
    /// <param name="builder">The host builder to register services with.</param>
    /// <param name="registry">The providers the host opted into.</param>
    /// <returns>The same <paramref name="builder"/>, for chaining.</returns>
    /// <exception cref="InvalidOperationException">The <c>DatabasePlatform</c> configuration section is missing or invalid.</exception>
    /// <exception cref="NotSupportedException">A configured database platform has no registered provider.</exception>
    private static IHostApplicationBuilder AddDatabaseProviderRegistration(
        this IHostApplicationBuilder builder,
        DatabaseProviderRegistry registry)
    {
        var databasePlatformOptions = GetSection<DatabasePlatformOptions>(builder.Configuration)
            .Get<DatabasePlatformOptions>()
            ?? throw new InvalidOperationException("Missing or invalid 'DatabasePlatform' configuration section.");

        var queryDatabaseProvider = ResolveDatabaseProvider(registry, databasePlatformOptions.QueryDbPlatform, "Query");
        var commandDatabaseProvider = ResolveDatabaseProvider(registry, databasePlatformOptions.CommandDbPlatform, "Command");

//#if (HasDapper)
        builder.AddDapperPersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);
//#endif
//#if (HasEfCore)
        builder.AddEFCorePersistenceRegistrations(queryDatabaseProvider, commandDatabaseProvider);
//#endif

        return builder;
    }

    /// <summary>
    /// Resolves the provider registered for a configured platform name, prefixing any failure with
    /// the CQRS side so a misconfiguration is attributable at a glance.
    /// </summary>
    /// <param name="registry">The providers the host opted into.</param>
    /// <param name="platform">The platform name from configuration; case-insensitive.</param>
    /// <param name="side">"Query" or "Command"; used only in the error message.</param>
    /// <returns>The provider for <paramref name="platform"/>.</returns>
    /// <exception cref="NotSupportedException">
    /// <paramref name="platform"/> is missing or has no registered provider. The message lists the
    /// registered platforms.
    /// </exception>
    private static IDatabaseProvider ResolveDatabaseProvider(DatabaseProviderRegistry registry, string platform, string side)
    {
        try
        {
            return registry.GetProvider(platform);
        }
        catch (Exception ex) when (ex is NotSupportedException or ArgumentException)
        {
            throw new NotSupportedException($"{side} database platform could not be resolved: {ex.Message}", ex);
        }
    }
//#if (HasDapper)

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
//#if (IncludeSamples)

        builder.Services.AddScoped<ISampleEntityDapperQueryRepository, SampleEntityDapperQueryRepository>();
        builder.Services.AddScoped<ISampleEntityDapperCommandRepository, SampleEntityDapperCommandRepository>();
//#endif

        return builder;
    }
//#endif
//#if (HasEfCore)

    /// <summary>
    /// Registers the EF Core path: <see cref="CommandDbContext"/> and <see cref="QueryDbContext"/>
    /// configured for their providers (with detailed errors and sensitive data logging outside
    /// Production), maps <see cref="ICommandDbContext"/>, <see cref="IUnitOfWork"/>, and
    /// <see cref="IQueryDbContext"/> onto those scoped context instances, and registers
    /// <see cref="IBulkCommandDbContext"/> over the same command context, bound to the command-side
    /// provider so it knows whether entity-list bulk operations are available.
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
        builder.Services.AddScoped<IBulkCommandDbContext>(sp =>
            new EfCoreBulkCommandDbContext(sp.GetRequiredService<CommandDbContext>(), commandDatabaseProvider));

        return builder;
    }
//#endif

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