using Cfo.Cats.Domain.HelpLinks.Events;

namespace Cfo.Cats.Application.Features.HelpLinks.EventHandlers;

public class HelpLinkDeletedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<HelpLinkDeletedDomainEvent>
{
    public Task Handle(HelpLinkDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        unitOfWork.DbContext.HelpLinks.Remove(@event.Entity);
        return Task.CompletedTask;
    }
}
