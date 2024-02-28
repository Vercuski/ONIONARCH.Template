using Microsoft.Data.SqlClient;
using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions;

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

    Task<int> ExecuteSqlAsync(string sql, IEnumerable<SqlParameter> parameters, CancellationToken cancellationToken = default);
}