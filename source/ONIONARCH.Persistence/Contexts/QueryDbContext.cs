
using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions.Context;
using System.Reflection;

namespace ONIONARCH.Persistence.Contexts;

public sealed class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : BaseDbContext<QueryDbContext>(options), IQueryDbContext
{
    IQueryable<TEntity> IQueryDbContext.Set<TEntity>()
    {
        return base.Set<TEntity>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }
}
