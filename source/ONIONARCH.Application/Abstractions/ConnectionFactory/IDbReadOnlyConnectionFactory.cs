using System.Data;

namespace ONIONARCH.Application.Abstractions.ConnectionFactory;

/// <summary>
/// Creates ADO.NET connections to the read (query) database.
/// </summary>
/// <remarks>
/// Consumed only by Persistence-layer Dapper query repositories. Request handlers must not
/// depend on this factory directly — the architecture fitness tests require query handlers to
/// take <see cref="Context.IQueryDbContext"/> or a query repository port instead, so raw SQL never
/// runs inside the Application layer.
/// </remarks>
public interface IDbReadOnlyConnectionFactory
{
    /// <summary>
    /// Creates a new, unopened connection to the query database using the configured
    /// provider and connection string.
    /// </summary>
    /// <returns>A new <see cref="IDbConnection"/>; the caller owns it and must dispose it.</returns>
    IDbConnection CreateConnection();
}
