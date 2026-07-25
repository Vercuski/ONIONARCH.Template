using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Options;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

public class SqlDbWriteConnectionFactory(IOptions<ConnectionStringOptions> connectionStringOptions) : IDbWriteConnectionFactory
{
    private readonly string _connectionString = connectionStringOptions.Value.CommandDbConnection;

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
