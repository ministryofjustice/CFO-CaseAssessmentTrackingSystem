using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
public class AllActivitiesAdvancedSpecification : Specification<Activity>
{
    public AllActivitiesAdvancedSpecification(AllActivitiesAdvancedFilter filter)
    {
        var requiresQa = ActivityDefinition.List
            .Where(def => def.RequiresQa)
            .Select(def => def.Type)
            .Distinct()
            .ToList();

        var includeTypes = filter.IncludeTypes?.Select(ActivityType.FromValue).ToList();

        Query.Where(a => a.Participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
            .Where(a => requiresQa.Contains(a.Type))
            .Where(a => a.TenantId.StartsWith(filter.UserProfile.TenantId!))
            .Where(a => a.TenantId.StartsWith(filter.TenantId!), string.IsNullOrEmpty(filter.TenantId) == false)
            .Where(a => a.OwnerId == filter.OwnerId, string.IsNullOrEmpty(filter.OwnerId) == false)
            .Where(a => a.TookPlaceAtLocation.Id == filter.LocationId, filter.LocationId is not null)
            .Where(a => includeTypes!.Contains(a.Type), includeTypes is { Count: > 0 })
            .Where(a => a.Status == ActivityStatus.FromValue(filter.Status!.Value), filter.Status is not null)
            .Where(a => string.IsNullOrWhiteSpace(filter.Keyword)
                        || a.Participant.FirstName.Contains(filter.Keyword)
                        || a.Participant.LastName.Contains(filter.Keyword)
                        || a.ParticipantId.Contains(filter.Keyword));
    }
}
