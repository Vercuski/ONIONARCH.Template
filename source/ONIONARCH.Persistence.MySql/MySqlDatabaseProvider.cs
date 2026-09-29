using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.MySql;

/// <summary>
/// <see cref="IDatabaseProvider"/> for MySQL and MariaDB. Both the EF Core path (via the
/// Microting.EntityFrameworkCore.MySql provider) and the Dapper path use MySqlConnector, so the
/// platform runs on a single ADO.NET driver. Internal so hosts can only obtain it through
/// <see cref="DependencyInjection.AddMySql"/>.
/// </summary>
/// <param name="serverVersion">
/// The server version EF Core generates SQL for. Supplied explicitly rather than auto-detected, because
/// <c>ServerVersion.AutoDetect</c> opens a database connection just to configure a DbContext.
/// </param>
internal sealed class MySqlDatabaseProvider(ServerVersion serverVersion) : IDatabaseProvider
{
    /// <summary>
    /// The <c>DatabasePlatform</c> configuration value that selects this provider.
    /// </summary>
    public const string PlatformKey = "MySQL";

    /// <inheritdoc />
    public string Platform => PlatformKey;

    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
}
