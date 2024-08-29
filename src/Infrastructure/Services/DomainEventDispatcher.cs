using Cfo.Cats.Domain.Common.Contracts;
using MediatR;

namespace Cfo.Cats.Infrastructure.Services;

public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(IApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        var domainEntities = context.ChangeTracker
            .Entries<IEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        while (domainEvents.Any())
        {
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }

            domainEntities = context.ChangeTracker
                      .Entries<IEntity>()
                      .Where(x => x.Entity.DomainEvents.Any())
                      .ToList();

            domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();
        }


    }
}
