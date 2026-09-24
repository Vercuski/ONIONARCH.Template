using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Abstractions.Repositories;

/// <summary>
/// Read-side port for the Dapper persistence path. Defined in Application, implemented in
/// Persistence — mirrors <see cref="Context.IQueryDbContext"/> for the EF Core path so that
/// Application never depends on Dapper, raw SQL, or <see cref="System.Data.IDbConnection"/>.
/// </summary>
public interface ISampleEntityDapperQueryRepository
{
    /// <summary>
    /// Retrieves every sample entity from the query database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>All sample entities (empty if none exist).</returns>
    Task<List<SampleEntityDefinition>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single sample entity by key.
    /// </summary>
    /// <param name="sampleId">The key of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none was found.</returns>
    Task<SampleEntityDefinition?> GetByIdAsync(int sampleId, CancellationToken cancellationToken = default);
}
