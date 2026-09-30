using ONIONARCH.Domain.Abstractions;
using System.Linq.Expressions;

namespace ONIONARCH.Application.Abstractions.Context;

/// <summary>
/// Declares the column assignments of a set-based update issued through
/// <see cref="IBulkCommandDbContext.UpdateWhereAsync{TEntity}"/>. Each call adds one <c>SET</c> clause.
/// </summary>
/// <remarks>
/// Mirrors EF Core's <c>UpdateSettersBuilder&lt;T&gt;</c> so the Application layer can describe an
/// update without referencing EF Core. Calls may be made conditionally (e.g. only set a column when a
/// request field is supplied); the Persistence implementation forwards each one as it is made.
/// </remarks>
/// <typeparam name="TEntity">The domain entity type being updated.</typeparam>
public interface IBulkUpdateSetters<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Sets <paramref name="property"/> to a constant <paramref name="value"/> (sent as a parameter).
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">Selects the mapped property to set, e.g. <c>e =&gt; e.Name</c>.</param>
    /// <param name="value">The value to assign.</param>
    /// <returns>This instance, for chaining.</returns>
    IBulkUpdateSetters<TEntity> Set<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        TProperty value);

    /// <summary>
    /// Sets <paramref name="property"/> to a value computed in SQL from the current row,
    /// e.g. <c>e =&gt; e.Count + 1</c>.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="property">Selects the mapped property to set.</param>
    /// <param name="value">An expression over the current row that must be translatable to SQL.</param>
    /// <returns>This instance, for chaining.</returns>
    IBulkUpdateSetters<TEntity> Set<TProperty>(
        Expression<Func<TEntity, TProperty>> property,
        Expression<Func<TEntity, TProperty>> value);
}
