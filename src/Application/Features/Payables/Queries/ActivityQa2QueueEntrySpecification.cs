using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Queries
{
    public class ActivityQa2QueueEntrySpecification : Specification<ActivityQa2QueueEntry>
    {
        public ActivityQa2QueueEntrySpecification(ActivityQueueEntryFilter filter)
        {
            Query.Where(e => e.TenantId
                    .StartsWith(filter.CurrentUser!.TenantId!))
                .Where(e => e.IsCompleted == false);
        }
    }
}