using Microsoft.EntityFrameworkCore.Query;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using System.Linq.Expressions;

namespace ONIONARCH.Persistence.Bulk;

/// <summary>
/// Adapts the Application-layer <see cref="IBulkUpdateSetters{TEntity}"/> to EF Core's
/// <see cref="UpdateSettersBuilder{TSource}"/>. Assignments are recorded first and replayed onto
/// EF Core's builder when <c>ExecuteUpdateAsync</c> runs, so an update with no assignments can be
/// rejected before any SQL is generated.
/// </summary>
/// <typeparam name="TEntity">The domain entity type being updated.</typeparam>
internal sealed class EfCoreBulkUpdateSetters<TEntity> : IBulkUpdateSetters<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// The recorded assignments, in the order they were declared.
    /// </summary>
    private readonly List<Action<UpdateSettersBuilder<TEntity>>> assignments = [];

    /// <summary>
    /// Gets the number of recorded assignments.
    /// </summary>
    public int Count => assignments.Count;

    /// <inheritdoc />
    public IBulkUpdateSetters<TEntity> Set<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        TProperty value)
    {
        ArgumentNullException.ThrowIfNull(property);
        assignments.Add(builder => builder.SetProperty(property, value));
        return this;
    }

    /// <inheritdoc />
    public IBulkUpdateSetters<TEntity> Set<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        Expression<Func<TEntity, TProperty>> value)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(value);
        assignments.Add(builder => builder.SetProperty(property, value));
        return this;
    }

    /// <summary>
    /// Replays every recorded assignment onto EF Core's builder.
    /// </summary>
    /// <param name="builder">The builder EF Core passes to <c>ExecuteUpdateAsync</c>.</param>
    public void ApplyTo(UpdateSettersBuilder<TEntity> builder)
    {
        foreach (var assignment in assignments)
        {
            assignment(builder);
        }
    }
}
