using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Options;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

public sealed class DbWriteConnectionFactory(
    IOptions<ConnectionStringOptions> connectionStringOptions,
    IDatabaseProvider databaseProvider) : IDbWriteConnectionFactory
{
    private readonly string _connectionString = connectionStringOptions.Value.CommandDbConnection;

    public IDbConnection CreateConnection() =>
        databaseProvider.CreateConnection(_connectionString);
}