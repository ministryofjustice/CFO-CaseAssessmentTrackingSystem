using Cfo.Cats.Domain.Entities.Notifications;

namespace Cfo.Cats.Domain.Events;

public sealed class NotificationCreatedDomainEvent(Notification notification) : DomainEvent
{
    public Notification Item {get;} = notification;
}