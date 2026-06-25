using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
public class AllActivitiesAdvancedSpecification : Specification<Activity>
{
    public AllActivitiesAdvancedSpecification(AllActivitiesAdvancedFilter filter)
    {
        var typeFilter = filter.TypeFilter.HasValue ? ActivityType.FromValue(filter.TypeFilter.Value) : null;

        Query.Where(a => a.Participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
            .Where(a => a.TenantId.StartsWith(filter.UserProfile.TenantId!))
            .Where(a => a.TenantId.StartsWith(filter.TenantId!), string.IsNullOrEmpty(filter.TenantId) == false)
            .Where(a => a.OwnerId == filter.OwnerId, string.IsNullOrEmpty(filter.OwnerId) == false)
            .Where(a => a.TookPlaceAtLocation.Id == filter.LocationId, filter.LocationId is not null)
            .Where(a => a.Type == typeFilter, typeFilter is not null)
            .Where(a => a.Status == ActivityStatus.FromValue(filter.Status!.Value), filter.Status is not null)
            .Where(a => string.IsNullOrWhiteSpace(filter.Keyword)
                        || a.Participant.FirstName.Contains(filter.Keyword)
                        || a.Participant.LastName.Contains(filter.Keyword)
                        || a.ParticipantId.Contains(filter.Keyword));
    }
}
