//#if (HasMySql && HasEfCore)
using Microsoft.Extensions.Configuration;
//#endif
//#if (HasMySql)
using ONIONARCH.Persistence.MySql;
//#endif
//#if (HasPostgreSql)
using ONIONARCH.Persistence.PostgreSql;
//#endif
using ONIONARCH.Persistence.Providers;
//#if (HasSqlServer)
using ONIONARCH.Persistence.SqlServer;
//#endif
//#if (HasMySql && HasEfCore)
using MySqlRegistration = ONIONARCH.Persistence.MySql.DependencyInjection;
//#endif

namespace ONIONARCH.Tests.PersistenceTests;

/// <summary>
/// Unit tests for <see cref="DatabaseProviderRegistry"/> and the provider registration extensions.
/// </summary>
[TestFixture]
public class DatabaseProviderRegistryTests
{
    /// <summary>
    /// Verifies that a registered platform resolves regardless of the configured value's casing.
    /// </summary>
    [Test]
    public void GetProvider_Should_ResolveRegisteredPlatform_CaseInsensitively()
    {
        var provider = FakeProvider("MSSQL");
        var registry = new DatabaseProviderRegistry().Add(provider);

        Assert.That(registry.GetProvider("mssql"), Is.SameAs(provider));
    }

    /// <summary>
    /// Verifies that an unregistered platform throws, and that the message names both the requested
    /// platform and the registered ones so the misconfiguration is obvious.
    /// </summary>
    [Test]
    public void GetProvider_Should_ThrowNotSupported_ListingRegisteredPlatforms_WhenPlatformIsUnregistered()
    {
        var registry = new DatabaseProviderRegistry().Add(FakeProvider("MSSQL"));

        var ex = Assert.Throws<NotSupportedException>(() => registry.GetProvider("PostgreSQL"));

        Assert.That(ex!.Message, Does.Contain("PostgreSQL").And.Contain("MSSQL"));
    }

    /// <summary>
    /// Verifies that resolving from an empty registry throws and reports that none are registered.
    /// </summary>
    [Test]
    public void GetProvider_Should_ThrowNotSupported_WhenNoProvidersAreRegistered()
    {
        var ex = Assert.Throws<NotSupportedException>(() => new DatabaseProviderRegistry().GetProvider("MSSQL"));

        Assert.That(ex!.Message, Does.Contain("none"));
    }

    /// <summary>
    /// Verifies that registering a second provider for a platform key that differs only by case is rejected.
    /// </summary>
    [Test]
    public void Add_Should_Throw_WhenPlatformIsAlreadyRegistered_IgnoringCase()
    {
        var registry = new DatabaseProviderRegistry().Add(FakeProvider("MySQL"));

        Assert.Throws<InvalidOperationException>(() => registry.Add(FakeProvider("MYSQL")));
    }

    /// <summary>
    /// Verifies the platform keys each provider registers. These keys are the contract with the
    /// <c>DatabasePlatform</c> section of every host's appsettings.json; renaming one silently breaks
    /// configuration.
    /// </summary>
    [Test]
    public void ProviderRegistrations_Should_ExposeTheConfiguredPlatformKeys()
    {
        var registry = new DatabaseProviderRegistry();
        var expectedPlatforms = new List<string>();

//#if (HasSqlServer)
        registry.AddSqlServer();
        expectedPlatforms.Add("MSSQL");
//#endif
//#if (HasPostgreSql)
        registry.AddPostgreSql();
        expectedPlatforms.Add("PostgreSQL");
//#endif
//#if (HasMySql)
//#if (HasEfCore)
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [MySqlRegistration.ServerVersionConfigurationKey] = "8.4.0-mysql"
            })
            .Build();
        registry.AddMySql(configuration);
//#else
//~        registry.AddMySql();
//#endif
        expectedPlatforms.Add("MySQL");
//#endif

        Assert.That(registry.Platforms, Is.EquivalentTo(expectedPlatforms));
    }

    /// <summary>
    /// Creates a fake <see cref="IDatabaseProvider"/> reporting <paramref name="platform"/>.
    /// </summary>
    /// <param name="platform">The platform key the fake reports.</param>
    /// <returns>The fake provider.</returns>
    private static IDatabaseProvider FakeProvider(string platform)
    {
        var provider = A.Fake<IDatabaseProvider>();
        A.CallTo(() => provider.Platform).Returns(platform);
        return provider;
    }
}
