using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public sealed class ParticipantsArchivedResultsAdvancedSpecification : Specification<ParticipantEnrolmentHistory>
{
    public ParticipantsArchivedResultsAdvancedSpecification(ParticipantsArchivedResultsAdvancedFilter filter)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        Query.Where(peh => peh.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value
                          && peh.From >= thirtyDaysAgo);

        Query.Where(peh => peh.CreatedBy == filter.CurrentUser!.UserId, filter.JustMyParticipants);
             
             //.Where(peh => peh.Participant!.Owner!.TenantId!.StartsWith(filter.CurrentUser!.TenantId!));

        // If we have passed a filter through, search the surname and current location
        Query.Where(peh => string.IsNullOrWhiteSpace(filter.Keyword)
                            || peh.Participant!.CurrentLocation.Name.Contains(filter.Keyword)
                            || peh.Participant!.FirstName.Contains(filter.Keyword)
                            || peh.Participant!.LastName.Contains(filter.Keyword)
                            || peh.ParticipantId.Contains(filter.Keyword));             
    }
}