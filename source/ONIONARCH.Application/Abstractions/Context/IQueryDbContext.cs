using Microsoft.EntityFrameworkCore;
using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions.Context;

public interface IQueryDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity;
}