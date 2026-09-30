//#if (HasEfCore)
using Microsoft.EntityFrameworkCore;
//#endif
//#if (HasDapper)
using System.Data;
//#endif

namespace ONIONARCH.Persistence.Providers;

/// <summary>
/// Abstracts a database platform so the EF Core and Dapper paths can be pointed at any supported
/// platform through configuration alone. Implemented by the provider-specific projects
/// (ONIONARCH.Persistence.SqlServer, .PostgreSql, .MySql); this core project owns the contract but
/// never references a concrete provider. Hosts opt providers in through
/// <see cref="DatabaseProviderRegistry"/>.
/// </summary>
public interface IDatabaseProvider
{
    /// <summary>
    /// The platform key matched, case-insensitively, against the <c>QueryDbPlatform</c> and
    /// <c>CommandDbPlatform</c> values of the <c>DatabasePlatform</c> configuration section
    /// (e.g. <c>MSSQL</c>, <c>PostgreSQL</c>, <c>MySQL</c>).
    /// </summary>
    string Platform { get; }
//#if (HasEfCore)

    /// <summary>
    /// Configures <paramref name="optionsBuilder"/> to use this platform's EF Core provider.
    /// </summary>
    /// <param name="optionsBuilder">The EF Core options builder to configure.</param>
    /// <param name="connectionString">The connection string to use.</param>
    void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString);

    /// <summary>
    /// Gets a value indicating whether this platform supports the entity-list bulk operations of
    /// <c>IBulkCommandDbContext</c> (insert, update, delete, upsert). They are implemented with
    /// EFCore.BulkExtensions, which needs a platform adapter package; a provider returns
    /// <see langword="true"/> only when its project references that adapter. Set-based
    /// <c>UpdateWhereAsync</c>/<c>DeleteWhereAsync</c> use EF Core itself and work regardless.
    /// </summary>
    bool SupportsBulkOperations { get; }
//#endif
//#if (HasDapper)

    /// <summary>
    /// Creates a new, unopened ADO.NET connection for this platform.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    /// <returns>A new connection; the caller owns it and must dispose it.</returns>
    IDbConnection CreateConnection(string connectionString);
//#endif
}
