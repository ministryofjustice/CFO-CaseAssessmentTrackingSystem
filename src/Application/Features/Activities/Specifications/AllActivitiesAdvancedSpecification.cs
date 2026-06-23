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

        Query.Where(a => a.Participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
            .Where(a => requiresQa.Contains(a.Type))
            .Where(a => a.TenantId.StartsWith(filter.UserProfile.TenantId!))
            .Where(a => a.TenantId.StartsWith(filter.TenantId!), string.IsNullOrEmpty(filter.TenantId) == false)
            .Where(a => a.OwnerId == filter.OwnerId, string.IsNullOrEmpty(filter.OwnerId) == false)
            .Where(a => a.TookPlaceAtLocation.Id == filter.Location!.Id, filter.Location is not null)
            .Where(a => filter.IncludeTypes!.Contains(a.Type), filter.IncludeTypes is { Count: > 0 })
            .Where(a => a.Status == filter.Status, filter.Status is not null)
            .Where(a => string.IsNullOrWhiteSpace(filter.Keyword)
                        || a.Participant.FirstName.Contains(filter.Keyword)
                        || a.Participant.LastName.Contains(filter.Keyword)
                        || a.ParticipantId.Contains(filter.Keyword));
    }
}
