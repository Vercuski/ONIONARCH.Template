using Microsoft.EntityFrameworkCore;
using ONIONARCH.Domain.Entities;

namespace ONIONARCH.Persistence.Contexts;

/// <summary>
/// Shared EF Core model for the command and query contexts. Declare entity sets here so both
/// sides of the CQRS split map the same schema.
/// </summary>
/// <typeparam name="T">
/// The concrete derived context type, so each derived context receives its own strongly typed
/// <see cref="DbContextOptions{TContext}"/> from DI.
/// </typeparam>
/// <param name="options">The options for the derived context.</param>
public abstract class BaseDbContext<T>(DbContextOptions<T> options) : DbContext(options) where T : DbContext
{
    /// <summary>
    /// Gets or sets the set of <see cref="SampleEntityDefinition"/> entities.
    /// </summary>
    public DbSet<SampleEntityDefinition> SampleEntity { get; set; }
}
