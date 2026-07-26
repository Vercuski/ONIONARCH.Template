using Microsoft.EntityFrameworkCore.ChangeTracking;
using ONIONARCH.Domain.Abstractions;
using System.Data;

namespace ONIONARCH.Application.Abstractions.Context;

public interface ICommandDbContext
{
    EntityEntry<TEntity> Alter<TEntity>(TEntity entity)
        where TEntity : Entity;

    EntityEntry<TEntity> Insert<TEntity>(TEntity entity)
        where TEntity : Entity;

    void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity;

    EntityEntry<TEntity> Delete<TEntity>(TEntity entity)
        where TEntity : Entity;

    int SaveChanges();

    Task<int> ExecuteSqlAsync(string sql, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken = default);
}