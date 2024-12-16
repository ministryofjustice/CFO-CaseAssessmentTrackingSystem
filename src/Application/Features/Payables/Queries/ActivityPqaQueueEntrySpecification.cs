using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Queries
{
    public class ActivityPqaQueueEntrySpecification : Specification<ActivityPqaQueueEntry>
    {
        public ActivityPqaQueueEntrySpecification(ActivityQueueEntryFilter filter)
        {
            Query.Where(e => e.TenantId
                    .StartsWith(filter.CurrentUser!.TenantId!))
                .Where(e => e.IsCompleted == false);
        }
    }
}