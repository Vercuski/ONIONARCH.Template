using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// <see cref="IDatabaseProvider"/> for Microsoft SQL Server, using Microsoft.Data.SqlClient for
/// both EF Core and Dapper connections.
/// </summary>
public sealed class SqlServerDatabaseProvider : IDatabaseProvider
{
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