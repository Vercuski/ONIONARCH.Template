//#if (HasEfCore)
using Microsoft.EntityFrameworkCore;
//#endif
//#if (HasDapper)
using Npgsql;
//#endif
using ONIONARCH.Persistence.Providers;
//#if (HasDapper)
using System.Data;
//#endif

namespace ONIONARCH.Persistence.PostgreSql;

/// <summary>
/// <see cref="IDatabaseProvider"/> for PostgreSQL, using Npgsql for both EF Core and Dapper connections.
/// Internal so hosts can only obtain it through <see cref="DependencyInjection.AddPostgreSql"/>.
/// </summary>
internal sealed class PostgreSqlDatabaseProvider : IDatabaseProvider
{
    /// <summary>
    /// The <c>DatabasePlatform</c> configuration value that selects this provider.
    /// </summary>
    public const string PlatformKey = "PostgreSQL";

    /// <inheritdoc />
    public string Platform => PlatformKey;
//#if (HasEfCore)

    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
//#endif
//#if (HasDapper)

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }
//#endif
}
