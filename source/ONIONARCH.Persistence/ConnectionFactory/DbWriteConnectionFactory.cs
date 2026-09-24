using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

/// <summary>
/// Creates connections to the command database using
/// <see cref="ConnectionStringOptions.CommandDbConnection"/> and the configured command-side
/// <see cref="IDatabaseProvider"/>.
/// </summary>
/// <param name="connectionStringOptions">The bound connection string settings.</param>
/// <param name="databaseProvider">The provider for the configured command database platform.</param>
public sealed class DbWriteConnectionFactory(
    IOptions<ConnectionStringOptions> connectionStringOptions,
    IDatabaseProvider databaseProvider) : IDbWriteConnectionFactory
{
    /// <summary>
    /// The command database connection string, captured at construction.
    /// </summary>
    private readonly string _connectionString = connectionStringOptions.Value.CommandDbConnection;

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        return databaseProvider.CreateConnection(_connectionString);
    }
}