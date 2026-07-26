using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

public sealed class DbReadOnlyConnectionFactory(
    IOptions<ConnectionStringOptions> connectionStringOptions,
    IDatabaseProvider databaseProvider) : IDbReadOnlyConnectionFactory
{
    private readonly string _connectionString = connectionStringOptions.Value.QueryDbConnection;

    public IDbConnection CreateConnection()
    {
        return databaseProvider.CreateConnection(_connectionString);
    }
}