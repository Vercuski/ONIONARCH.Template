using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// Abstracts a database platform (SQL Server, PostgreSQL, MySQL) so the EF Core and Dapper paths
/// can be pointed at any supported platform through configuration alone.
/// </summary>
public interface IDatabaseProvider
{
    /// <summary>
    /// Configures <paramref name="optionsBuilder"/> to use this platform's EF Core provider.
    /// </summary>
    /// <param name="optionsBuilder">The EF Core options builder to configure.</param>
    /// <param name="connectionString">The connection string to use.</param>
    void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString);

    /// <summary>
    /// Creates a new, unopened ADO.NET connection for this platform.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    /// <returns>A new connection; the caller owns it and must dispose it.</returns>
    IDbConnection CreateConnection(string connectionString);
}