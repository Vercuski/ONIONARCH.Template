using System.Data;

namespace ONIONARCH.Application.Abstractions.ConnectionFactory;
public interface IDbConnectionFactory
{
    IDbConnection CreateReadConnection();
    IDbConnection CreateWriteConnection();
}
