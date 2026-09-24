using ONIONARCH.Domain.Abstractions;

namespace ONIONARCH.Application.Abstractions;

/// <summary>
/// Implemented by inbound DTOs that know how to convert themselves into a domain entity.
/// </summary>
/// <typeparam name="TEntity">The domain entity type the DTO maps to.</typeparam>
public interface IDomainMapper<out TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Creates a new domain entity populated from this DTO's values.
    /// </summary>
    /// <returns>A new <typeparamref name="TEntity"/> instance.</returns>
    TEntity MapToDomain();
}
