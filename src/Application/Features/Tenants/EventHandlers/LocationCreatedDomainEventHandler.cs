using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Tenants.EventHandlers;

public class LocationCreatedDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<LocationCreatedDomainEvent>
{
    public async Task Handle(LocationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var query = unitOfWork.DbContext.Contracts.Include(c => c.Tenant)
            .Where(c => c.Id == notification.ContractId)
            .Select(c => c.Tenant);

        var tenant = await query.SingleAsync(cancellationToken);
        tenant!.AddLocation(notification.Entity);
    }
}