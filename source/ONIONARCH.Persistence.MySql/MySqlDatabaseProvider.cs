//#if (HasEfCore)
using Microsoft.EntityFrameworkCore;
//#endif
//#if (HasDapper)
using MySqlConnector;
//#endif
using ONIONARCH.Persistence.Providers;
//#if (HasDapper)
using System.Data;
//#endif

namespace ONIONARCH.Persistence.MySql;

/// <summary>
/// <see cref="IDatabaseProvider"/> for MySQL and MariaDB. Both the EF Core path (via the
/// Microting.EntityFrameworkCore.MySql provider) and the Dapper path use MySqlConnector, so the
/// platform runs on a single ADO.NET driver. Internal so hosts can only obtain it through
/// <see cref="DependencyInjection.AddMySql"/>.
/// </summary>
//#if (HasEfCore)
/// <param name="serverVersion">
/// The server version EF Core generates SQL for. Supplied explicitly rather than auto-detected, because
/// <c>ServerVersion.AutoDetect</c> opens a database connection just to configure a DbContext.
/// </param>
//#endif
//#if (HasEfCore)
internal sealed class MySqlDatabaseProvider(ServerVersion serverVersion) : IDatabaseProvider
//#else
//~internal sealed class MySqlDatabaseProvider : IDatabaseProvider
//#endif
{
    /// <summary>
    /// The <c>DatabasePlatform</c> configuration value that selects this provider.
    /// </summary>
    public const string PlatformKey = "MySQL";

    /// <inheritdoc />
    public string Platform => PlatformKey;
//#if (HasEfCore)

    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }

    /// <inheritdoc />
    /// <remarks>
    /// <see langword="false"/>: EFCore.BulkExtensions publishes no EF Core 10 adapter for MySQL
    /// (<c>EFCore.BulkExtensions.MySql</c> stops at 9.x, built on Pomelo/EF Core 9, which conflicts with the
    /// Microting EF Core 10 provider used here). Revisit when an EF Core 10 MySQL adapter ships.
    /// </remarks>
    public bool SupportsBulkOperations => false;
//#endif
//#if (HasDapper)

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
//#endif
}
