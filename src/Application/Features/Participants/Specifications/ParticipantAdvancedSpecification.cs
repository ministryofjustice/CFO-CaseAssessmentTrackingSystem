using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public sealed class ParticipantAdvancedSpecification : Specification<Participant>
{
    public ParticipantAdvancedSpecification(ParticipantAdvancedFilter filter)
    {
        
        Query.Where( p => p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value 
                          && p.EnrolmentStatus != EnrolmentStatus.DormantStatus.Value, 
                            filter.ListView == ParticipantListView.Default);
        
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus.Value, filter.ListView == ParticipantListView.Identified);
        Query.Where(p => p.EnrolmentStatus == EnrolmentStatus.EnrollingStatus.Value, filter.ListView == ParticipantListView.Enrolling);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value, filter.ListView == ParticipantListView.SubmittedToProvider);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value, filter.ListView == ParticipantListView.SubmittedToQa);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.SubmittedToProviderStatus.Value || p.EnrolmentStatus == EnrolmentStatus.SubmittedToAuthorityStatus.Value, filter.ListView == ParticipantListView.SubmittedToAny);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value, filter.ListView == ParticipantListView.Approved);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.DormantStatus.Value, filter.ListView == ParticipantListView.Dormant);
        Query.Where( p => p.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value, filter.ListView == ParticipantListView.Archived);
        
       
        Query.Where(p => p.OwnerId == filter.CurrentUser!.UserId, filter.JustMyCases)
             .Where(p => p.Owner!.TenantId!.StartsWith(filter.CurrentUser!.TenantId!))
             .Where(p => filter.Locations.Contains(p.CurrentLocation.Id) ||  ( p.EnrolmentLocation != null && filter.Locations.Contains(p.EnrolmentLocation.Id)) , filter.Locations is not [])
            .Where(
                    // if we have passed a filter through, search the surname and current location
                    p => p.LastName!.Contains(filter.Keyword!) 
                         || p.CurrentLocation.Name.Contains(filter.Keyword!) 
                         || p.Id.Contains(filter.Keyword!), 
                    string.IsNullOrEmpty(filter.Keyword) == false);
            
    }

    
}
