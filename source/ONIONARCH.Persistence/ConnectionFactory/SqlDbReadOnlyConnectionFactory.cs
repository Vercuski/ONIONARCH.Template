using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Options;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

public class SqlDbReadOnlyConnectionFactory(IOptions<ConnectionStringOptions> connectionStringOptions) : IDbReadOnlyConnectionFactory
{
    private readonly string _connectionString = connectionStringOptions.Value.QueryDbConnection;

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
