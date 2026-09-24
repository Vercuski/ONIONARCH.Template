using Microsoft.Extensions.Options;
using ONIONARCH.Application.Abstractions.ConnectionFactory;
using ONIONARCH.Persistence.Options;
using ONIONARCH.Persistence.Providers;
using System.Data;

namespace ONIONARCH.Persistence.ConnectionFactory;

/// <summary>
/// Creates connections to the query database using
/// <see cref="ConnectionStringOptions.QueryDbConnection"/> and the configured query-side
/// <see cref="IDatabaseProvider"/>.
/// </summary>
/// <param name="connectionStringOptions">The bound connection string settings.</param>
/// <param name="databaseProvider">The provider for the configured query database platform.</param>
public sealed class DbReadOnlyConnectionFactory(
    IOptions<ConnectionStringOptions> connectionStringOptions,
    IDatabaseProvider databaseProvider) : IDbReadOnlyConnectionFactory
{
    /// <summary>
    /// The query database connection string, captured at construction.
    /// </summary>
    private readonly string _connectionString = connectionStringOptions.Value.QueryDbConnection;

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        return databaseProvider.CreateConnection(_connectionString);
    }
}