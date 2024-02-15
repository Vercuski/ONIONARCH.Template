using ONIONARCH.Domain.Abstractions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ONIONARCH.Application.Abstractions;

public interface IQueryDbContext
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity;
}