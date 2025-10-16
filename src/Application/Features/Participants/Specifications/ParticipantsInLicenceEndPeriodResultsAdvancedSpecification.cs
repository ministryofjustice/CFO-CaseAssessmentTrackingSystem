using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public sealed class ParticipantsInLicenceEndPeriodResultsAdvancedSpecification : Specification<Participant>
{
    public ParticipantsInLicenceEndPeriodResultsAdvancedSpecification(ParticipantsInLicenceEndPeriodResultsAdvancedFilter filter)
    {
        var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

        Query.Where(p => p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                          && p.EnrolmentStatus != EnrolmentStatus.DormantStatus.Value
                          && p.DeactivatedInFeed >= thirtyDaysAgo);
                             
        Query.Where(p => p.OwnerId == filter.CurrentUser!.UserId, filter.JustMyParticipants)
             .Where(p => p.Owner!.TenantId!.StartsWith(filter.CurrentUser!.TenantId!));

        // if we have passed a filter through, search the surname and current location
        Query.Where(p => string.IsNullOrWhiteSpace(filter.Keyword)
                            || p.CurrentLocation.Name.Contains(filter.Keyword)
                            || p.FirstName.Contains(filter.Keyword)
                            || p.LastName.Contains(filter.Keyword)
                            || p.Id.Contains(filter.Keyword));             
    }    
}