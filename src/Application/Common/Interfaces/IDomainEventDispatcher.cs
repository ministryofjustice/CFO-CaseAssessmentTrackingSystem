namespace Cfo.Cats.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IApplicationDbContext context, CancellationToken cancellationToken = default);
}
