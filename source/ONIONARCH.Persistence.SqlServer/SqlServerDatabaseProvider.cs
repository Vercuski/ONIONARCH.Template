using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.SqlServer;

/// <summary>
/// <see cref="IDatabaseProvider"/> for Microsoft SQL Server, using Microsoft.Data.SqlClient for
/// both EF Core and Dapper connections.
/// Internal so hosts can only obtain it through <see cref="DependencyInjection.AddSqlServer"/>.
/// </summary>
internal sealed class SqlServerDatabaseProvider : IDatabaseProvider
{
    /// <summary>
    /// The <c>DatabasePlatform</c> configuration value that selects this provider.
    /// </summary>
    public const string PlatformKey = "MSSQL";

    /// <inheritdoc />
    public string Platform => PlatformKey;

    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
}
