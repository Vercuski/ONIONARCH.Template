
using Microsoft.EntityFrameworkCore;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using System.Reflection;

namespace ONIONARCH.Persistence.Contexts;

/// <summary>
/// EF Core context for the read side, configured for no-tracking queries.
/// </summary>
/// <param name="options">The options configured for the query database.</param>
public sealed class QueryDbContext(DbContextOptions<QueryDbContext> options)
    : BaseDbContext<QueryDbContext>(options), IQueryDbContext
{
    /// <inheritdoc />
    /// <remarks>
    /// Implemented explicitly so the <see cref="IQueryDbContext"/> surface returns
    /// <see cref="IQueryable{T}"/> instead of EF Core's <c>DbSet&lt;T&gt;</c>, keeping EF Core
    /// types out of the Application layer.
    /// </remarks>
    IQueryable<TEntity> IQueryDbContext.Set<TEntity>()
    {
        return base.Set<TEntity>();
    }

    /// <inheritdoc />
    public Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
    where TEntity : Entity
    {
        return query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TEntity?> SingleOrDefaultAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        return query.SingleOrDefaultAsync(cancellationToken);
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

    /// <summary>
    /// Sets <see cref="QueryTrackingBehavior.NoTracking"/> as the default for this context, since
    /// entities read here are never saved back through it.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to configure the context.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }
}
