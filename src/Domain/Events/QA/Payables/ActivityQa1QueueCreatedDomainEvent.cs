using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Payables
{
    public sealed class ActivityQa1QueueCreatedDomainEvent(ActivityQa1QueueEntry entity) : CreatedDomainEvent<ActivityQa1QueueEntry>(entity)
    {
    }
}