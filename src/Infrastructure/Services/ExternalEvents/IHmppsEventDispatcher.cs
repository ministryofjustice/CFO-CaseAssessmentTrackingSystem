using Cfo.Cats.Application.Features.HmppsExternalApi.ExternalEvents;

namespace Cfo.Cats.Infrastructure.Services.ExternalEvents;

/// <summary>
/// Routes an <see cref="HmppsNotification"/> to the <see cref="IHmppsEventHandler"/> registered for
/// its event type.
/// </summary>
public interface IHmppsEventDispatcher
{
    Task Dispatch(HmppsNotification notification, CancellationToken cancellationToken);
}
