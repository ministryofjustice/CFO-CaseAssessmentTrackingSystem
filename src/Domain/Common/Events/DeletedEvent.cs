using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Events;

public class DeletedEvent<T> : DomainEvent
    where T : IEntity
{
    public DeletedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
