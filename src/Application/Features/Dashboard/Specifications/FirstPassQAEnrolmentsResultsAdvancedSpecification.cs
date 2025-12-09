using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Dashboard.Specifications;

public sealed class FirstPassQAEnrolmentsResultsAdvancedSpecification : Specification<Participant>
{
    public FirstPassQAEnrolmentsResultsAdvancedSpecification(FirstPassQAEnrolmentsResultsAdvancedFilter filter)
    {
        Query.Where(p => p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value);
   
        Query.Where(p => p.Owner!.TenantId!.StartsWith(filter.CurrentUser!.TenantId!));

        // If we have passed a filter through, search the surname and current location
        Query.Where(p => string.IsNullOrWhiteSpace(filter.Keyword)
                         || p.CurrentLocation.Name.Contains(filter.Keyword)
                         || p.FirstName.Contains(filter.Keyword)
                         || p.LastName.Contains(filter.Keyword)
                         || p.Id.Contains(filter.Keyword));             
    }
}