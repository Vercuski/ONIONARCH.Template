using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Persistence.Providers;
using System.Linq.Expressions;

namespace ONIONARCH.Persistence.Bulk;

/// <summary>
/// EF Core implementation of <see cref="IBulkCommandDbContext"/>. Entity-list operations are delegated
/// to EFCore.BulkExtensions; set-based updates and deletes use EF Core's own
/// <c>ExecuteUpdateAsync</c>/<c>ExecuteDeleteAsync</c> (which EFCore.BulkExtensions itself defers to).
/// </summary>
/// <remarks>
/// <para>
/// This core project references only <c>EFCore.BulkExtensions.Core</c>, which is provider-agnostic. The
/// platform adapter (<c>EFCore.BulkExtensions.SqlServer</c>, <c>.PostgreSql</c>) is referenced by the
/// matching provider project and located by the library at runtime, so this project stays free of
/// provider-specific assemblies.
/// </para>
/// <para>
/// Works on the scoped <see cref="CommandDbContext"/>, so every operation runs on the same connection as
/// <see cref="ICommandDbContext"/> and enlists in a transaction begun through
/// <c>IUnitOfWork.BeginTransactionAsync</c>. Without one, EFCore.BulkExtensions wraps multi-step
/// operations (e.g. staging-table merges) in its own transaction.
/// </para>
/// </remarks>
/// <param name="context">The scoped command context.</param>
/// <param name="databaseProvider">The provider for the command database.</param>
internal sealed class EfCoreBulkCommandDbContext(CommandDbContext context, IDatabaseProvider databaseProvider)
    : IBulkCommandDbContext
{
    /// <inheritdoc />
    public Task BulkInsertAsync<TEntity>(
        IReadOnlyCollection<TEntity> entities,
        bool retrieveGeneratedKeys = false,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return RunAsync(entities, list =>
            context.BulkInsertAsync(list, CreateConfig(retrieveGeneratedKeys), cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task BulkUpdateAsync<TEntity>(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return RunAsync(entities, list =>
            context.BulkUpdateAsync(list, CreateConfig(), cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    public Task BulkDeleteAsync<TEntity>(IReadOnlyCollection<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return RunAsync(entities, list =>
            context.BulkDeleteAsync(list, CreateConfig(), cancellationToken: cancellationToken));
    }

    /// <inheritdoc />
    /// <remarks>
    /// Entities whose store-generated key is still unset (e.g. <c>0</c> for an identity column) are sent
    /// through <c>BulkInsert</c> and only the rest through <c>BulkInsertOrUpdate</c>, both in one
    /// transaction. Passing unsaved entities to <c>BulkInsertOrUpdate</c> is not portable: the PostgreSQL
    /// adapter writes the unset key literally (inserting a row with key <c>0</c>, then updating that row on
    /// the next upsert) where SQL Server's <c>MERGE</c> lets the database generate it.
    /// </remarks>
    public Task BulkUpsertAsync<TEntity>(
        IReadOnlyCollection<TEntity> entities,
        bool retrieveGeneratedKeys = false,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return RunAsync(entities, async list =>
        {
            var (unsaved, saved) = SplitByGeneratedKey(list);
            await InTransactionAsync(async () =>
            {
                if (unsaved.Count > 0)
                {
                    await context.BulkInsertAsync(unsaved, CreateConfig(retrieveGeneratedKeys), cancellationToken: cancellationToken);
                }
                if (saved.Count > 0)
                {
                    await context.BulkInsertOrUpdateAsync(saved, CreateConfig(), cancellationToken: cancellationToken);
                }
            }, cancellationToken);
        });
    }

    /// <inheritdoc />
    public Task<int> UpdateWhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        Action<IBulkUpdateSetters<TEntity>> setters,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(setters);

        var recorded = new EfCoreBulkUpdateSetters<TEntity>();
        setters(recorded);
        if (recorded.Count == 0)
        {
            throw new ArgumentException("A bulk update must set at least one property.", nameof(setters));
        }

        return context.Set<TEntity>().Where(predicate).ExecuteUpdateAsync(recorded.ApplyTo, cancellationToken);
    }

    /// <inheritdoc />
    public Task<int> DeleteWhereAsync<TEntity>(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return context.Set<TEntity>().Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Creates the EFCore.BulkExtensions configuration shared by every entity-list operation. This is the
    /// single place to tune the library (batch size, timeouts, SQL Server bulk-copy options, and so on).
    /// </summary>
    /// <param name="retrieveGeneratedKeys">Whether to read store-generated keys back into the entities.</param>
    /// <returns>A new configuration.</returns>
    private static BulkConfig CreateConfig(bool retrieveGeneratedKeys = false)
    {
        return new BulkConfig
        {
            SetOutputIdentity = retrieveGeneratedKeys
        };
    }

    /// <summary>
    /// Splits <paramref name="entities"/> into those whose store-generated primary key is still unset
    /// (EF Core's own sentinel check, as used by <c>SaveChanges</c>) and all others. An entity type whose
    /// key is not store-generated has no unsaved entities: every key is significant.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities to split.</param>
    /// <returns>The unsaved entities and the entities with a key.</returns>
    /// <exception cref="InvalidOperationException"><typeparamref name="TEntity"/> is not in the model or has no primary key.</exception>
    private (List<TEntity> Unsaved, List<TEntity> Saved) SplitByGeneratedKey<TEntity>(IList<TEntity> entities)
        where TEntity : Entity
    {
        var key = context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()
            ?? throw new InvalidOperationException($"'{typeof(TEntity).Name}' must be in the command model and have a primary key to be upserted.");

        if (key.Properties is not [var keyProperty] || !keyProperty.ValueGenerated.HasFlag(ValueGenerated.OnAdd))
        {
            return ([], [.. entities]);
        }

        var getter = keyProperty.GetGetter();
        var unsaved = new List<TEntity>();
        var saved = new List<TEntity>();
        foreach (var entity in entities)
        {
            (getter.HasSentinelValueUsingContainingEntity(entity) ? unsaved : saved).Add(entity);
        }
        return (unsaved, saved);
    }

    /// <summary>
    /// Runs <paramref name="work"/> in the ambient transaction if there is one, otherwise in a new
    /// transaction committed on success, so multi-call operations stay atomic.
    /// </summary>
    /// <param name="work">The operations to run.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the work has been committed.</returns>
    private async Task InTransactionAsync(Func<Task> work, CancellationToken cancellationToken)
    {
        if (context.Database.CurrentTransaction is not null)
        {
            await work();
            return;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await work();
        await transaction.CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Guards an entity-list operation: validates the argument, rejects platforms without a bulk adapter,
    /// and skips the round trip for an empty collection.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities passed by the caller.</param>
    /// <param name="operation">The EFCore.BulkExtensions call to run.</param>
    /// <returns>A task that completes when the operation has finished.</returns>
    /// <exception cref="NotSupportedException">The command database platform has no bulk adapter.</exception>
    private Task RunAsync<TEntity>(IReadOnlyCollection<TEntity> entities, Func<IList<TEntity>, Task> operation)
        where TEntity : Entity
    {
        ArgumentNullException.ThrowIfNull(entities);

        if (!databaseProvider.SupportsBulkOperations)
        {
            throw new NotSupportedException(
                $"Entity-list bulk operations are not available on the '{databaseProvider.Platform}' command database: " +
                "EFCore.BulkExtensions has no adapter for it. UpdateWhereAsync and DeleteWhereAsync are still supported.");
        }

        return entities.Count == 0
            ? Task.CompletedTask
            : operation(entities as IList<TEntity> ?? [.. entities]);
    }
}
