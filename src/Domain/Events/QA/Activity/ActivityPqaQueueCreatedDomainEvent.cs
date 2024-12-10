using Cfo.Cats.Domain.Common.Events;
using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Domain.Events.QA.Activity;
public sealed class ActivityPqaQueueCreatedDomainEvent(ActivityPqaQueueEntry entity) : CreatedDomainEvent<ActivityPqaQueueEntry>(entity)
{
}