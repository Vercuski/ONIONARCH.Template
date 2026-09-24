using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Persistence.Transactions;
using System.Data;
using System.Reflection;

namespace ONIONARCH.Persistence.Contexts;

/// <summary>
/// EF Core context for the write side. Implements both <see cref="ICommandDbContext"/> and
/// <see cref="IUnitOfWork"/>, and both are resolved to the same scoped instance so they share
/// change tracking within a request.
/// </summary>
/// <param name="options">The options configured for the command database.</param>
public sealed class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : BaseDbContext<CommandDbContext>(options), ICommandDbContext, IUnitOfWork
{
    /// <inheritdoc />
    public void Insert<TEntity>(TEntity entity) where TEntity : Entity
    {
        Set<TEntity>().Add(entity);
    }

    /// <inheritdoc />
    public void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities) where TEntity : Entity
    {
        Set<TEntity>().AddRange(entities);
    }

    /// <inheritdoc />
    public void Alter<TEntity>(TEntity entity) where TEntity : Entity
    {
        Set<TEntity>().Update(entity);
    }

    /// <inheritdoc />
    public void Delete<TEntity>(TEntity entity) where TEntity : Entity
    {
        Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken = default)
    {
        return Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }

    /// <inheritdoc cref="IUnitOfWork.SaveChangesAsync" />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken);
        return new EfCoreUnitOfWorkTransaction(transaction);
    }

    /// <summary>
    /// Applies every <c>IEntityTypeConfiguration&lt;T&gt;</c> defined in the Persistence assembly
    /// before completing EF Core's default model configuration.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
