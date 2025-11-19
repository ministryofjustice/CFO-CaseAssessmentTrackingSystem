using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Dashboard.Specifications;

public sealed class FirstPassQAActivitiesResultsAdvancedSpecification : Specification<Participant>
{
    public FirstPassQAActivitiesResultsAdvancedSpecification(FirstPassQAActivitiesResultsAdvancedFilter filter)
    {
        Query.Where(p => p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
        //                   && peh.From >= thirtyDaysAgo);
        //
        // Query.Where(peh => peh.Participant!.OwnerId == filter.CurrentUser!.UserId, filter.JustMyParticipants);
        //      
        // Query.Where(peh => peh.Participant!.Owner!.TenantId!.StartsWith(filter.CurrentUser!.TenantId!));
        //
        // If we have passed a filter through, search the surname and current location
        Query.Where(p => string.IsNullOrWhiteSpace(filter.Keyword)
                            || p.CurrentLocation.Name.Contains(filter.Keyword)
                            || p.FirstName.Contains(filter.Keyword)
                            || p.LastName.Contains(filter.Keyword)
                            || p.Id.Contains(filter.Keyword));             
    }
}