namespace Cfo.Cats.Domain.Common.Contracts;

public interface IEntity
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

public interface IEntity<TId> : IEntity
{
    TId Id { get; set; }
}
