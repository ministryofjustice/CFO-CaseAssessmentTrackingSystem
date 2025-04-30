using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class ActivityQaEscalationQueueEntrySpecification : Specification<ActivityEscalationQueueEntry>
{
    public ActivityQaEscalationQueueEntrySpecification(ActivityQueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);     

        Query.Where(e => 
            e.Participant!.Id.Contains(filter.Keyword!) || e.Participant!.LastName.Contains(filter.Keyword!),
        string.IsNullOrEmpty(filter.Keyword) == false);       
    }
}