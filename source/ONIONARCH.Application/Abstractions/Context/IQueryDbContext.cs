using Microsoft.EntityFrameworkCore;
using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions.Context;

public interface IQueryDbContext
{
    IQueryable<TEntity> Set<TEntity>()
        where TEntity : Entity;
}