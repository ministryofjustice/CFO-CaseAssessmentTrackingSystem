using Cfo.Cats.Domain.Labels.Events;

namespace Cfo.Cats.Application.Features.Labels.EventHandlers;

public class LabelDeletedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<LabelDeletedDomainEvent>
{
    public Task Handle(LabelDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        unitOfWork.DbContext.Labels.Remove(@event.Entity);    
        return Task.CompletedTask;
    }
}