using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Tenants.EventHandlers
{
    public class LocationCreatedDomainEventHandler : INotificationHandler<LocationCreatedDomainEvent>
    {
        private IApplicationDbContext _context;

        public LocationCreatedDomainEventHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(LocationCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var query = _context.Contracts.Include(c => c.Tenant)
                            .Where(c => c.Id == notification.ContractId)
                            .Select(c => c.Tenant);

            var tenant = await query.SingleAsync(cancellationToken);
            tenant!.AddLocation(notification.Entity);
        }
    }
}