using MediatR;

namespace Cfo.Cats.Domain.Common;

public abstract class DomainEvent : INotification
{
    protected DomainEvent()
    {
        DateOccurred = DateTimeOffset.Now;
    }

    public bool IsPublished { get; set; }
    public DateTimeOffset DateOccurred { get; protected set; }
}
