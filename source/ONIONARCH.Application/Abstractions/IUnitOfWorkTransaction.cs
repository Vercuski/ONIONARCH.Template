namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// An explicit database transaction started by <see cref="IUnitOfWork.BeginTransactionAsync"/>.
/// </summary>
/// <remarks>
/// Dispose the transaction (<c>await using</c>) when finished. Disposing without calling
/// <see cref="CommitAsync"/> discards the transaction's work.
/// </remarks>
public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    /// <summary>
    /// Commits all work performed within the transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the commit has finished.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back all work performed within the transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the rollback has finished.</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}