using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Payables
{    
    public sealed class ActivityQa2QueueCreatedDomainEvent(ActivityQa2QueueEntry entity) : CreatedDomainEvent<ActivityQa2QueueEntry>(entity)
    {
    }
}
