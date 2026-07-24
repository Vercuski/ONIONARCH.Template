using ONIONARCH.Domain.Abstractions;
using System.Data;

namespace ONIONARCH.Application.Abstractions.Context;

public interface ICommandDbContext
{
    void Alter<TEntity>(TEntity entity)
        where TEntity : Entity;

    void Insert<TEntity>(TEntity entity)
        where TEntity : Entity;

    void InsertRange<TEntity>(IReadOnlyCollection<TEntity> entities)
        where TEntity : Entity;

    void Remove<TEntity>(TEntity entity)
        where TEntity : Entity;

    int SaveChanges();

    Task<int> ExecuteSqlAsync(string sql, IEnumerable<IDataParameter> parameters, CancellationToken cancellationToken = default);
}