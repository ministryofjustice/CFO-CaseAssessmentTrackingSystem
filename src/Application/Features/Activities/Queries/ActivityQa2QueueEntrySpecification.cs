using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries
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