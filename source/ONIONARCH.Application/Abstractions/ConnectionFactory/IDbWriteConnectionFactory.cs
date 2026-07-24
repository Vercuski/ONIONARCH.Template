using System.Data;

namespace ONIONARCH.Application.Abstractions.ConnectionFactory;

public interface IDbWriteConnectionFactory
{
    IDbConnection CreateConnection();
}
