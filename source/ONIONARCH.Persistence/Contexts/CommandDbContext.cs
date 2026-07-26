using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using System.Data;
using System.Reflection;

namespace ONIONARCH.Persistence.Contexts;

public sealed class CommandDbContext(DbContextOptions<CommandDbContext> options)
    : BaseDbContext<CommandDbContext>(options), ICommandDbContext, IUnitOfWork
{
    public EntityEntry<TEntity> Insert<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        return Set<TEntity>().Add(entity);
    }

    public void  InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity
    {
         Set<TEntity>().AddRange(entities);
    }

    public EntityEntry<TEntity> Alter<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        return Set<TEntity>().Update(entity);
    }

    public EntityEntry<TEntity> Delete<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        return Set<TEntity>().Remove(entity);
    }

    public Task<int> ExecuteSqlAsync(string sql, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken = default)
    {
        return Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
