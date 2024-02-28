using Microsoft.EntityFrameworkCore;
using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions;

public interface IQueryDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity;
}