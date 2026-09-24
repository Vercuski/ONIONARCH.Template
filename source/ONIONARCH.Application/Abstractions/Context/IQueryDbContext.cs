using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions.Context;

/// <summary>
/// Read-side port for the EF Core persistence path. Defined in Application and implemented by
/// Persistence's <c>QueryDbContext</c> (configured for no-tracking queries).
/// </summary>
/// <remarks>
/// Exposes <see cref="IQueryable{T}"/> rather than EF Core's <c>DbSet&lt;T&gt;</c> so the
/// Application layer never references EF Core. Because EF Core's async LINQ operators live in
/// EF Core itself, the async materialization methods are surfaced here instead
/// (<see cref="ToListAsync{TEntity}"/>, <see cref="SingleOrDefaultAsync{TEntity}"/>).
/// </remarks>
public interface IQueryDbContext
{
    /// <summary>
    /// Returns a composable query over all entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type to query.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> that can be further filtered and projected before execution.</returns>
    IQueryable<TEntity> Set<TEntity>() where TEntity : Entity;

    /// <summary>
    /// Asynchronously executes <paramref name="query"/> and materializes the results into a list.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="query">A query obtained from <see cref="Set{TEntity}"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list containing every entity matched by the query (empty if none).</returns>
    Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Asynchronously executes <paramref name="query"/> and returns its single result, or
    /// <see langword="null"/> if it matched nothing.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="query">A query obtained from <see cref="Set{TEntity}"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The matching entity, or <see langword="null"/> if none was found.</returns>
    /// <exception cref="InvalidOperationException">The query matched more than one entity.</exception>
    Task<TEntity?> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
        where TEntity : Entity;
}