using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions;

public interface IDomainMapper<out TEntity>
    where TEntity : Entity
{
    TEntity MapToDomain();
}
