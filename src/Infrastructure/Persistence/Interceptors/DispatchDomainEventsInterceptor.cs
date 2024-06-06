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

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var domainEventEntities = context!.ChangeTracker
            .Entries<IEntity>()
            .Select(po => po.Entity)
            .Where(po => po.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEventEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        if (domainEvents.Any())
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var saveResult = await base.SavingChangesAsync(eventData, result, cancellationToken);

                foreach (var entity in domainEventEntities)
                {
                    entity.ClearDomainEvents();
                }

                foreach (var e in domainEvents)
                {
                    await mediator.Publish(e, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return saveResult;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
