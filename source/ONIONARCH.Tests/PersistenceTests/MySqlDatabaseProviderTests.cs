using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ONIONARCH.Persistence.MySql;
using ONIONARCH.Persistence.Providers;
using MySqlRegistration = ONIONARCH.Persistence.MySql.DependencyInjection;

namespace ONIONARCH.Tests.PersistenceTests;

/// <summary>
/// Unit tests for the MySQL provider's registration and configuration. The provider type is internal,
/// so every test reaches it the way a host does: through <c>AddMySql</c> and the registry.
/// </summary>
[TestFixture]
public class MySqlDatabaseProviderTests
{
    /// <summary>
    /// A syntactically valid connection string. No test opens a connection.
    /// </summary>
    private const string ConnectionString = "Server=localhost;Database=Sample;User ID=user;Password=pass";

    /// <summary>
    /// Verifies that registration fails at startup, naming the configuration key, when no server
    /// version is configured.
    /// </summary>
    [Test]
    public void AddMySql_Should_Throw_WhenServerVersionIsMissing()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => new DatabaseProviderRegistry().AddMySql(Configuration(serverVersion: null)));

        Assert.That(ex!.Message, Does.Contain(MySqlRegistration.ServerVersionConfigurationKey));
    }

    /// <summary>
    /// Verifies that registration fails at startup, naming the key and the bad value, when the
    /// configured server version cannot be parsed.
    /// </summary>
    [Test]
    public void AddMySql_Should_Throw_WhenServerVersionIsInvalid()
    {
        var ex = Assert.Throws<InvalidOperationException>(
            () => new DatabaseProviderRegistry().AddMySql(Configuration("not-a-version")));

        Assert.That(ex!.Message,
            Does.Contain(MySqlRegistration.ServerVersionConfigurationKey).And.Contain("not-a-version"));
    }

    /// <summary>
    /// Verifies that the EF Core and Dapper paths both use MySqlConnector, so the platform runs on a
    /// single ADO.NET driver.
    /// </summary>
    [Test]
    public void MySqlProvider_Should_UseMySqlConnector_ForBothEfCoreAndDapper()
    {
        var provider = RegisteredProvider("8.4.0-mysql");

        using var context = CreateContext(provider);
        using var dapperConnection = provider.CreateConnection(ConnectionString);

        Assert.Multiple(() =>
        {
            Assert.That(context.Database.GetDbConnection(), Is.TypeOf<MySqlConnector.MySqlConnection>());
            Assert.That(dapperConnection, Is.TypeOf<MySqlConnector.MySqlConnection>());
        });
    }

    /// <summary>
    /// Verifies that EF Core is configured with exactly the configured server version, for both MySQL
    /// and MariaDB version strings, rather than an auto-detected or default one.
    /// </summary>
    /// <param name="serverVersion">The configured server version string.</param>
    [TestCase("8.4.0-mysql")]
    [TestCase("11.4.2-mariadb")]
    public void MySqlProvider_Should_ConfigureEfCore_WithTheConfiguredServerVersion(string serverVersion)
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        RegisteredProvider(serverVersion).ConfigureEfCore(optionsBuilder, ConnectionString);

        // Read via reflection: the provider's options extension lives in an .Internal namespace, and
        // referencing it directly would trip the EF1001 analyzer (an error under TreatWarningsAsErrors).
        var extension = optionsBuilder.Options.Extensions.Single(e => e.GetType().Name == "MySqlOptionsExtension");
        var configured = extension.GetType().GetProperty("ServerVersion")!.GetValue(extension);

        Assert.That(configured?.ToString(), Is.EqualTo(serverVersion));
    }

    /// <summary>
    /// Registers the MySQL provider with <paramref name="serverVersion"/> and resolves it.
    /// </summary>
    /// <param name="serverVersion">The server version to configure.</param>
    /// <returns>The registered provider.</returns>
    private static IDatabaseProvider RegisteredProvider(string serverVersion) =>
        new DatabaseProviderRegistry()
            .AddMySql(Configuration(serverVersion))
            .GetProvider("MySQL");

    /// <summary>
    /// Builds in-memory configuration holding the given server version, or no value when
    /// <paramref name="serverVersion"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="serverVersion">The server version value, or <see langword="null"/> to omit it.</param>
    /// <returns>The configuration.</returns>
    private static IConfiguration Configuration(string? serverVersion) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [MySqlRegistration.ServerVersionConfigurationKey] = serverVersion
            })
            .Build();

    /// <summary>
    /// Creates a bare <see cref="DbContext"/> configured by <paramref name="provider"/>. The context
    /// never connects; it is only inspected.
    /// </summary>
    /// <param name="provider">The provider that configures the context.</param>
    /// <returns>The context; the caller disposes it.</returns>
    private static DbContext CreateContext(IDatabaseProvider provider)
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        provider.ConfigureEfCore(optionsBuilder, ConnectionString);
        return new DbContext(optionsBuilder.Options);
    }
}
