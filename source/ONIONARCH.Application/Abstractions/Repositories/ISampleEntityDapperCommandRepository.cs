using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Application.Abstractions.Repositories;

/// <summary>
/// Write-side port for the Dapper persistence path. Defined in Application, implemented in
/// Persistence — mirrors <see cref="Context.ICommandDbContext"/> for the EF Core path so that
/// Application never depends on Dapper, raw SQL, or <see cref="System.Data.IDbConnection"/>.
/// </summary>
public interface ISampleEntityDapperCommandRepository
{
    /// <summary>
    /// Inserts <paramref name="entity"/> into the command database.
    /// </summary>
    /// <param name="entity">The entity to insert, including its <see cref="SampleEntityDefinition.SampleId"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> CreateAsync(SampleEntityDefinition entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the row whose key matches <paramref name="entity"/>'s
    /// <see cref="SampleEntityDefinition.SampleId"/> with the entity's current values.
    /// </summary>
    /// <param name="entity">The entity carrying the key and new values.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows affected (0 if no row matched).</returns>
    Task<int> UpdateAsync(SampleEntityDefinition entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the row with the given key.
    /// </summary>
    /// <param name="sampleId">The key of the row to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows affected (0 if no row matched).</returns>
    Task<int> DeleteAsync(int sampleId, CancellationToken cancellationToken = default);
}
