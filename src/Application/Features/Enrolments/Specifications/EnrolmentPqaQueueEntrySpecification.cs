using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Enrolments.Specifications;

public class EnrolmentPqaQueueEntrySpecification : Specification<EnrolmentPqaQueueEntry>
{
    public EnrolmentPqaQueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);
    }
}

public class EnrolmentQa1QueueEntrySpecification : Specification<EnrolmentQa1QueueEntry>
{
    public EnrolmentQa1QueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);
    }
}