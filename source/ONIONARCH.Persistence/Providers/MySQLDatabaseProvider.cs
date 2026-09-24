using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// <see cref="IDatabaseProvider"/> for MySQL. Uses the MySql.EntityFrameworkCore provider for
/// EF Core and MySqlConnector for Dapper connections.
/// </summary>
public sealed class MySQLDatabaseProvider : IDatabaseProvider
{
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