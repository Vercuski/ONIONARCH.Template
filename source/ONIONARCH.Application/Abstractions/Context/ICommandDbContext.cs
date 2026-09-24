using ONIONARCH.Domain.Abstractions;
using System.Data;

namespace ONIONARCH.Application.Abstractions.Context;

/// <summary>
/// Write-side port for the EF Core persistence path. Defined in Application and implemented by
/// Persistence's <c>CommandDbContext</c>, so command handlers can stage and save changes
/// without taking a dependency on EF Core types.
/// </summary>
/// <remarks>
/// The staging methods (<see cref="Insert{TEntity}"/>, <see cref="Alter{TEntity}"/>, etc.) only
/// track changes; nothing reaches the database until <see cref="SaveChanges"/> is called.
/// </remarks>
public interface ICommandDbContext
{
    /// <summary>
    /// Stages <paramref name="entity"/> for insertion on the next save.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entity">The entity to insert.</param>
    void Insert<TEntity>(TEntity entity) where TEntity : Entity;

    /// <summary>
    /// Stages every entity in <paramref name="entities"/> for insertion on the next save.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entities">The entities to insert.</param>
    void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : Entity;

    /// <summary>
    /// Stages <paramref name="entity"/> for update on the next save. All of its properties are
    /// marked as modified.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entity">The entity whose current values should be persisted.</param>
    void Alter<TEntity>(TEntity entity) where TEntity : Entity;

    /// <summary>
    /// Stages <paramref name="entity"/> for deletion on the next save.
    /// </summary>
    /// <typeparam name="TEntity">The domain entity type.</typeparam>
    /// <param name="entity">The entity to delete.</param>
    void Delete<TEntity>(TEntity entity) where TEntity : Entity;

    /// <summary>
    /// Synchronously persists all staged changes to the command database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    int SaveChanges();

    /// <summary>
    /// Executes a raw, parameterized SQL statement against the command database immediately,
    /// bypassing change tracking.
    /// </summary>
    /// <param name="sql">The SQL statement to execute. Use parameter placeholders; never concatenate user input.</param>
    /// <param name="parameters">The parameter values referenced by <paramref name="sql"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(string sql, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken = default);
}