using ONIONARCH.Domain.Abstractions;
using System.Linq.Expressions;

namespace ONIONARCH.Application.Abstractions.Context;

/// <summary>
/// Write-side port for bulk operations on the EF Core persistence path. Defined in Application and
/// implemented in Persistence, so command handlers can write many rows in a few round trips without
/// depending on EF Core or on the bulk library behind it.
/// </summary>
/// <remarks>
/// <para>
/// Two kinds of operation are offered:
/// </para>
/// <list type="bullet">
///   <item><b>Entity-list operations</b> (<see cref="BulkInsertAsync{TEntity}"/>, <see cref="BulkUpdateAsync{TEntity}"/>,
///   <see cref="BulkDeleteAsync{TEntity}"/>, <see cref="BulkUpsertAsync{TEntity}"/>) take the entities to write and
///   match existing rows by primary key. They load the data through the platform's bulk-copy mechanism.
///   Not every database platform supports them; see the exception documentation.</item>
///   <item><b>Set-based operations</b> (<see cref="UpdateWhereAsync{TEntity}"/>, <see cref="DeleteWhereAsync{TEntity}"/>)
///   take a predicate and issue a single <c>UPDATE</c> or <c>DELETE</c> without loading any entities. They work
///   on every platform.</item>
/// </list>
/// <para>
/// Every method executes immediately and bypasses the change tracker: nothing is staged, entities already
/// tracked in the same scope are not refreshed (a later save can overwrite a bulk write with stale values),
/// and concurrency tokens are not checked. Avoid mixing tracked and bulk writes to the same rows.
/// </para>
/// <para>
/// All methods share the scoped command connection, so they enlist in a transaction begun through
/// <see cref="IUnitOfWork.BeginTransactionAsync"/>. Without one, each call is atomic on its own.
/// </para>
/// </remarks>
public interface IBulkCommandDbContext
{
    /// <summary>
    /// Inserts <paramref name="entities"/> using the platform's bulk-copy mechanism.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities to insert. An empty collection is a no-op.</param>
    /// <param name="retrieveGeneratedKeys">
    /// When <see langword="true"/>, store-generated keys (identity/serial columns) are read back into the
    /// entities. This costs an extra step (a staging table on SQL Server), so leave it off unless the
    /// caller needs the keys.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when every entity has been inserted.</returns>
    /// <exception cref="NotSupportedException">The command database platform does not support entity-list bulk operations.</exception>
    Task BulkInsertAsync<TEntity>(
        IReadOnlyCollection<TEntity> entities,
        bool retrieveGeneratedKeys = false,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Updates the rows matching <paramref name="entities"/> by primary key, writing every non-key column
    /// from each entity. Entities with no matching row are ignored.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities carrying the keys and new values. An empty collection is a no-op.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the update has finished.</returns>
    /// <exception cref="NotSupportedException">The command database platform does not support entity-list bulk operations.</exception>
    Task BulkUpdateAsync<TEntity>(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Deletes the rows matching <paramref name="entities"/> by primary key. Entities with no matching row
    /// are ignored. Only the keys need to be populated.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities whose rows should be deleted. An empty collection is a no-op.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the delete has finished.</returns>
    /// <exception cref="NotSupportedException">The command database platform does not support entity-list bulk operations.</exception>
    Task BulkDeleteAsync<TEntity>(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Inserts or updates <paramref name="entities"/> by primary key: rows that exist are updated, the rest
    /// are inserted.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities to write. An empty collection is a no-op.</param>
    /// <param name="retrieveGeneratedKeys">
    /// When <see langword="true"/>, keys generated for inserted rows are read back into the entities.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when every entity has been written.</returns>
    /// <exception cref="NotSupportedException">The command database platform does not support entity-list bulk operations.</exception>
    Task BulkUpsertAsync<TEntity>(
        IReadOnlyCollection<TEntity> entities,
        bool retrieveGeneratedKeys = false,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Updates every row matching <paramref name="predicate"/> with a single set-based <c>UPDATE</c>
    /// statement. No entities are loaded.
    /// </summary>
    /// <example>
    /// <code>
    /// await bulk.UpdateWhereAsync&lt;Order&gt;(
    ///     o =&gt; o.Status == OrderStatus.Pending &amp;&amp; o.CreatedUtc &lt; cutoff,
    ///     set =&gt; set
    ///         .Set(o =&gt; o.Status, OrderStatus.Expired)            // constant value
    ///         .Set(o =&gt; o.RetryCount, o =&gt; o.RetryCount + 1),    // computed from the current row
    ///     cancellationToken);
    /// </code>
    /// </example>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="predicate">Selects the rows to update. It must be translatable to SQL.</param>
    /// <param name="setters">Declares the columns to set and their new values. At least one is required.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows updated.</returns>
    /// <exception cref="ArgumentException"><paramref name="setters"/> declares no assignment.</exception>
    Task<int> UpdateWhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        Action<IBulkUpdateSetters<TEntity>> setters,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;

    /// <summary>
    /// Deletes every row matching <paramref name="predicate"/> with a single set-based <c>DELETE</c>
    /// statement. No entities are loaded.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="predicate">Selects the rows to delete. It must be translatable to SQL.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows deleted.</returns>
    Task<int> DeleteWhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;
}
