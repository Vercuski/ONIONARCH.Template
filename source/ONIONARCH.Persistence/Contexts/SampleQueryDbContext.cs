
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace ONIONARCH.Persistence.Contexts;

public sealed class SampleQueryDbContext(DbContextOptions options) : DbContext(options), IQueryDbContext, IUnitOfWork
{
    public new DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity
        => base.Set<TEntity>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
