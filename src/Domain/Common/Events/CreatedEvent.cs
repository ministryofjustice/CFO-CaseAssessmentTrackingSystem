using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Events;

public class CreatedEvent<T>(T entity) : DomainEvent
    where T : IEntity
{
    public T Entity { get; } = entity;
}
