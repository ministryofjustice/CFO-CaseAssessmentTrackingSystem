using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Entities.Activities;
using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public sealed record ActivityAbandonedIntegrationEvent(Activity item, DateTime dateTime) : IntegrationEvent
{
    public Activity Item { get; } = item;
    public DateTime DateTime { get; } = dateTime;
}