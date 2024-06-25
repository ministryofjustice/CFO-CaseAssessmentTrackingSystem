using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public sealed class ParticipantAdvancedSpecification : Specification<Participant>
{
    public ParticipantAdvancedSpecification(ParticipantAdvancedFilter filter)
    {
        EnrolmentStatus status = filter.ListView switch
        {
            ParticipantListView.Default => EnrolmentStatus.PendingStatus,
            ParticipantListView.Pending => EnrolmentStatus.PendingStatus,
            ParticipantListView.SubmittedToProvider => EnrolmentStatus.SubmittedToProviderStatus,
            ParticipantListView.SubmittedToQa => EnrolmentStatus.SubmittedToAuthorityStatus,
            ParticipantListView.SubmittedToAny => EnrolmentStatus.SubmittedToAuthorityStatus,
            ParticipantListView.Approved => EnrolmentStatus.ApprovedStatus,
            ParticipantListView.Abandoned => EnrolmentStatus.AbandonedStatus,
            ParticipantListView.All => EnrolmentStatus.PendingStatus,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Query.Where(p => p.OwnerId == filter.CurrentUser!.UserId)
            .Where(
            p => p.LastName!.Contains(filter.Keyword!)
                 || p.CurrentLocation.Name.Contains(filter.Keyword!)
            , string.IsNullOrEmpty(filter.Keyword) == false)
            .Where(p => p.EnrolmentStatus == status );
    }

    
}
