using System.ComponentModel.DataAnnotations.Schema;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Exceptions;

namespace Cfo.Cats.Domain.Common.Entities;

public abstract class BaseEntity<TId> : IEntity<TId>
{
    private readonly List<DomainEvent> domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public TId Id { get; set; } = default!;

    public void AddDomainEvent(DomainEvent domainEvent) => domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => domainEvents.Clear();

    protected void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}
