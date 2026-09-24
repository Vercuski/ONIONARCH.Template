using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// <see cref="IDatabaseProvider"/> for PostgreSQL, using Npgsql for both EF Core and Dapper connections.
/// </summary>
public sealed class PostgreSqlDatabaseProvider : IDatabaseProvider
{
    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }
}