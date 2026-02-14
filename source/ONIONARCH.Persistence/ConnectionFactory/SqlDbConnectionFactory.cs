using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Domain.Options;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;
public class SqlDbConnectionFactory(IOptions<ConnectionStringOptions> connectionStringOptions) : IDbConnectionFactory
{
    private readonly string _readConnectionString = connectionStringOptions.Value.QueryDbConnection;
    private readonly string _writeConnectionString = connectionStringOptions.Value.CommandDbConnection;

    public IDbConnection CreateReadConnection()
    {
        return new SqlConnection(_readConnectionString);
    }

    public IDbConnection CreateWriteConnection()
    {
        return new SqlConnection(_writeConnectionString);
    }
}
