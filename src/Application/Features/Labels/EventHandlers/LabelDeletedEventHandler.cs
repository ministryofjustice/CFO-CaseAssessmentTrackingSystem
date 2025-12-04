namespace Cfo.Cats.Domain.Labels.Events;

public class LabelDeletedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<LabelDeletedDomainEvent>
{
    public Task Handle(LabelDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        unitOfWork.DbContext.Labels.Remove(@event.Entity);    
        return Task.CompletedTask;
    }
}