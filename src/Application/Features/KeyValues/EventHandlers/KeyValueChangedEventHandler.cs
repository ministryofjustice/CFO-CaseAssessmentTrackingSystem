namespace Cfo.Cats.Application.Features.KeyValues.EventHandlers;

public class KeyValueChangedEventHandler(
    IPicklistService picklistService,
    ILogger<KeyValueChangedEventHandler> logger
) : INotificationHandler<UpdatedEvent<KeyValue>>
{

    public Task Handle(UpdatedEvent<KeyValue> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("KeyValue Changed {DomainEvent},{@Entity}", nameof(notification), notification.Entity);
        picklistService.Refresh();
        return Task.CompletedTask;
    }
}