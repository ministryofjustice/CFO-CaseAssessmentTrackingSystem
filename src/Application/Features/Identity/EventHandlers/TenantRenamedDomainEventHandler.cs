using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Identity.EventHandlers;

public class TenantRenamedDomainEventHandler(IUnitOfWork unitOfWork, ILogger<TenantRenamedDomainEventHandler> logger) : INotificationHandler<TenantRenamedDomainEvent>
{
    public async Task Handle(TenantRenamedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Tenant {notification.OldName} renamed to {notification.Entity.Name}. Updating users to match.");

        var result = await unitOfWork.DbContext.Users
            .Where(u => u.TenantId == notification.Entity.Id)
            .ExecuteUpdateAsync(u => u.SetProperty(user => user.TenantName, _ => notification.Entity.Name),
            cancellationToken: cancellationToken);

        logger.LogInformation($"Tenant {notification.OldName} renamed to {notification.Entity.Name}. Updated {result} rows.");
    }
}
