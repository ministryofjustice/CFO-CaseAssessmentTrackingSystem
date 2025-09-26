using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public sealed class ActivityAbandonedIntegrationEvent(Activity item, DateTime dateTime) : DomainEvent
{
    public Activity Item { get; } = item;
    public DateTime DateTime { get; } = dateTime;
}