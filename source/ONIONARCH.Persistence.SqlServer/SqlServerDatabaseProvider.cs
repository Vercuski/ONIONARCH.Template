//#if (HasDapper)
using Microsoft.Data.SqlClient;
//#endif
//#if (HasEfCore)
using Microsoft.EntityFrameworkCore;
//#endif
using ONIONARCH.Persistence.Providers;
//#if (HasDapper)
using System.Data;
//#endif

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
//#if (HasEfCore)

    /// <inheritdoc />
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }

    /// <inheritdoc />
    /// <remarks>Backed by the <c>EFCore.BulkExtensions.SqlServer</c> adapter this project references.</remarks>
    public bool SupportsBulkOperations => true;
//#endif
//#if (HasDapper)

    /// <inheritdoc />
    public IDbConnection CreateConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
    }
//#endif
}
