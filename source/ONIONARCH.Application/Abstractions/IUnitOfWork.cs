namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Unit-of-work port over the command database. Lets Application code commit staged changes
/// asynchronously and group multiple operations in an explicit transaction without depending
/// on EF Core.
/// </summary>
/// <remarks>
/// Implemented by Persistence's <c>CommandDbContext</c>, so within a DI scope it shares change
/// tracking with <see cref="Context.ICommandDbContext"/>.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously persists all staged changes to the command database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction on the command database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A transaction that must be committed or rolled back explicitly and then disposed.
    /// </returns>
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}