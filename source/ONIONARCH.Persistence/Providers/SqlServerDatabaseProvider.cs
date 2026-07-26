using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

public sealed class SqlServerDatabaseProvider : IDatabaseProvider
{
    public void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString) =>
        optionsBuilder.UseSqlServer(connectionString);

    public IDbConnection CreateConnection(string connectionString) =>
        new SqlConnection(connectionString);
}