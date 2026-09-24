using Microsoft.EntityFrameworkCore.Storage;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Persistence.Transactions;

/// <summary>
/// Adapts an EF Core <see cref="IDbContextTransaction"/> to the Application-layer
/// <see cref="IUnitOfWorkTransaction"/> port, so callers never see EF Core types.
/// </summary>
/// <param name="transaction">The underlying EF Core transaction.</param>
internal sealed class EfCoreUnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    /// <inheritdoc />
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the underlying EF Core transaction.
    /// </summary>
    /// <returns>A task that completes when disposal has finished.</returns>
    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }
}