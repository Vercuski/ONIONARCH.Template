using System.Data;

namespace ONIONARCH.Application.Abstractions.ConnectionFactory;

/// <summary>
/// Creates ADO.NET connections to the write (command) database.
/// </summary>
/// <remarks>
/// Consumed only by Persistence-layer Dapper command repositories. Request handlers must not
/// depend on this factory directly — the architecture fitness tests require command handlers to
/// take <see cref="Context.ICommandDbContext"/> or a command repository port instead, so raw SQL
/// never runs inside the Application layer.
/// </remarks>
public interface IDbWriteConnectionFactory
{
    /// <summary>
    /// Creates a new, unopened connection to the command database using the configured
    /// provider and connection string.
    /// </summary>
    /// <returns>A new <see cref="IDbConnection"/>; the caller owns it and must dispose it.</returns>
    IDbConnection CreateConnection();
}
