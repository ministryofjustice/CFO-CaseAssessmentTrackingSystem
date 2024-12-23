using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries
{
    public class ActivityQaEscalationQueueEntrySpecification : Specification<ActivityEscalationQueueEntry>
    {
        public ActivityQaEscalationQueueEntrySpecification(ActivityQueueEntryFilter filter)
        {
            Query.Where(e => e.TenantId
                    .StartsWith(filter.CurrentUser!.TenantId!))
                .Where(e => e.IsCompleted == false);            
        }
    }
}