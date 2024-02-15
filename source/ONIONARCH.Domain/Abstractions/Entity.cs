using System.Diagnostics.CodeAnalysis;

namespace ONIONARCH.Domain.Abstractions;

[ExcludeFromCodeCoverage]
public abstract class Entity : IEntity
{
    //private readonly List<DomainEvents> _domainEvents = new();

    //protected Entity(int id)
    //{
    //    Id = id;
    //}

    //protected Entity()
    //{
    //}

    //public int Id { get; init; }

    //public IReadOnlyCollection<DomainEvents> DomainEvents => _domainEvents.ToList();

    //public void ClearDomainEvents()
    //{
    //    _domainEvents.Clear();
    //}

    //protected void RaiseDomainEvent(DomainEvent domainEvent)
    //{
    //    _domainEvents.Add(domainEvent);
    //}
}
