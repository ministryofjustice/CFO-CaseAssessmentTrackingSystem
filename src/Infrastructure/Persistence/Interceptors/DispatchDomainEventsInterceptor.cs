#nullable disable warnings
using Cfo.Cats.Domain.Common.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cfo.Cats.Infrastructure.Persistence.Interceptors;

public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator mediator;

    public DispatchDomainEventsInterceptor(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        await DispatchDomainEventsForDelete(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default
    )
    {
        var resultValueTask = await base.SavedChangesAsync(eventData, result, cancellationToken);
        await DispatchDomainEventsForChanged(eventData.Context);
        return resultValueTask;
    }

    public async Task DispatchDomainEventsForDelete(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entities = context
            .ChangeTracker.Entries<IEntity>()
            .Where(e => e.Entity.DomainEvents.Any() && e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities.SelectMany(e => e.DomainEvents);

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }

    public async Task DispatchDomainEventsForChanged(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var entities = context
            .ChangeTracker.Entries<IEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
