using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ONIONARCH.Persistence.Providers;

public interface IDatabaseProvider
{
    void ConfigureEfCore(DbContextOptionsBuilder optionsBuilder, string connectionString);
    IDbConnection CreateConnection(string connectionString);
}