using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.MySql;

/// <summary>
/// <see cref="IDatabaseProvider"/> for MySQL. Uses the MySql.EntityFrameworkCore provider for
/// EF Core and MySqlConnector for Dapper connections.
/// Internal so hosts can only obtain it through <see cref="DependencyInjection.AddMySql"/>.
/// </summary>
internal sealed class MySqlDatabaseProvider : IDatabaseProvider
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
        optionsBuilder.UseMySQL(connectionString);
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new MySqlConnection(connectionString);
    }
}
