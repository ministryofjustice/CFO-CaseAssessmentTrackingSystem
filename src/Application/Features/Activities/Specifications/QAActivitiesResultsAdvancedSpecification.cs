using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
public class QAActivitiesResultsAdvancedSpecification : Specification<Activity>
{
    public QAActivitiesResultsAdvancedSpecification(QAActivitiesResultsAdvancedFilter filter)
    {
        var requiresQa = ActivityDefinition.List
            .Where(def => def.RequiresQa)
            .GroupBy(def => def.Type)
            .Select(def => def.Key)
            .ToList();

        Query.Where(a => a.Participant.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
             .Where(a => filter.IncludeTypes!.Contains(a.Type), filter.IncludeTypes is { Count: > 0 })
             .Where(a => a.CommencedOn >= filter.CommencedStart, filter.CommencedStart.HasValue)
             .Where(a => a.CommencedOn <= filter.CommencedEnd, filter.CommencedEnd.HasValue)
             .Where(a => a.TookPlaceAtLocation.Id == filter.Location!.Id, filter.Location is not null)             
             .Where(a =>
                a.Status == ActivityStatus.PendingStatus.Value ||
                (
                    filter.CommencedEnd.HasValue == false &&
                    a.CompletedOn >= DateTime.UtcNow.AddMonths(-1)
                )
             )
             .Where(a => a.Status == filter.Status, filter.Status is not null)
             .Where(a => a.TenantId.StartsWith(filter.UserProfile.TenantId!))
             .Where(a => a.OwnerId == filter.UserProfile.UserId, filter.JustMyParticipants)
             .Where(a => requiresQa.Contains(a.Type));

        Query
            .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
            .ThenByDescending(a => a.CompletedOn)
            .ThenBy(a => a.LastModified);
    }
}