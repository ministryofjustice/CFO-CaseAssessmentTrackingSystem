using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.Common.Events;

public class UpdatedEvent<T> : DomainEvent
    where T : IEntity
{
    public UpdatedEvent(T entity)
    {
        Entity = entity;
    }

    public T Entity { get; }
}
