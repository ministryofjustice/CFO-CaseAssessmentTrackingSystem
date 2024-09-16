using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

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

        Query.Where(e => e.ParticipantId.Contains(filter.Keyword!), string.IsNullOrWhiteSpace(filter.Keyword) == false);
    }
}

public class EnrolmentQa2QueueEntrySpecification : Specification<EnrolmentQa2QueueEntry>
{
    public EnrolmentQa2QueueEntrySpecification(QueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);
    }
}