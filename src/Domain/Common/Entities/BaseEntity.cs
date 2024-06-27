using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseEntity<TId> : IEntity<TId>
{
    private readonly List<DomainEvent> domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public virtual TId Id { get; set; } = default!;

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
