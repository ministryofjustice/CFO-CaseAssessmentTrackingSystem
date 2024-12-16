using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Queries
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