using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class ActivityPqaQueueEntrySpecification : Specification<ActivityPqaQueueEntry>
{
    public ActivityPqaQueueEntrySpecification(ActivityQueueEntryFilter filter)
    {
        Query.Where(e => e.TenantId
                .StartsWith(filter.CurrentUser!.TenantId!))
            .Where(e => e.IsCompleted == false);

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            if (filter.Keyword.Split(" ") is { Length: 2 } segments)
            {
                Query.Where(e => e.Participant!.FirstName.Contains(segments[0]) && e.Participant!.LastName.Contains(segments[1]));
            }
            else
            {
                Query.Where(e => 
                    e.Participant!.Id.Contains(filter.Keyword!)
                    || e.Participant!.FirstName.Contains(filter.Keyword!)
                    || e.Participant!.LastName.Contains(filter.Keyword!));
            }
        }

        Query.Where(e => e.Participant!.OwnerId == filter.SupportWorkerId!,
            !string.IsNullOrEmpty(filter.SupportWorkerId));
    }
}