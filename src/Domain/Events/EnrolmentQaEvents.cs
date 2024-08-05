using Cfo.Cats.Domain.Common.Events;

using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Domain.Events;

public sealed class EnrolmentPqaQueueCreatedDomainEvent(EnrolmentPqaQueueEntry entity) : CreatedDomainEvent<EnrolmentPqaQueueEntry>(entity)
{
}

public sealed class EnrolmentQa1QueueCreatedDomainEvent(EnrolmentQa1QueueEntry entity) : CreatedDomainEvent<EnrolmentQa1QueueEntry>(entity)
{
}

public sealed class EnrolmentQa2QueueCreatedDomainEvent(EnrolmentQa2QueueEntry entity) : CreatedDomainEvent<EnrolmentQa2QueueEntry>(entity)
{
}
