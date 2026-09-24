using System.Diagnostics.CodeAnalysis;

namespace ONIONARCH.Domain.Abstractions;

/// <summary>
/// Abstract base class for every persistable domain entity.
/// </summary>
/// <remarks>
/// Serves as the generic constraint (<c>where TEntity : Entity</c>) on the Application-layer
/// persistence abstractions (<c>ICommandDbContext</c>, <c>IQueryDbContext</c>,
/// <c>IDomainMapper&lt;TEntity&gt;</c>) so only true domain entities can flow through them.
/// The architecture fitness tests also require every type in <c>ONIONARCH.Domain.Entities</c>
/// to inherit from this class and be sealed.
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class Entity : IEntity
{
}
