using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class QAActivitiesResultsAdvancedSpecification : Specification<Activity>
{
    public QAActivitiesResultsAdvancedSpecification(QAActivitiesResultsAdvancedFilter filter)
    {
        var requiresQa = ActivityDefinition.List
            .Where(def => def.RequiresQa)
            .GroupBy(def => def.Type)
            .Select(def => def.Key)
            .ToArray();

        Query.Where(a => a.OwnerId == filter.CurentActiveUser.UserId)
             .Where(a => a.ParticipantId == filter.ParticipantId, filter.ParticipantId is not null)
             .Where(a => a.TaskId == filter.TaskId, filter.TaskId is not null)
             .Where(a => a.ObjectiveId == filter.ObjectiveId, filter.ObjectiveId is not null)
             .Where(a => a.TookPlaceAtLocation.Id == filter.Location!.Id, filter.Location is not null)
             .Where(a => filter.IncludeTypes!.Contains(a.Type), filter.IncludeTypes is { Count: > 0 })
             .Where(a => a.CommencedOn >= filter.CommencedStart, filter.CommencedStart is not null)
             .Where(a => a.CommencedOn <= filter.CommencedEnd, filter.CommencedEnd is not null)
             .Where(a => a.Status == filter.Status, filter.Status is not null)
             .Where(a => a.OwnerId == filter.CurentActiveUser.UserId
                && (
                    a.Status == ActivityStatus.PendingStatus.Value
                    || (
                        a.Status == ActivityStatus.ApprovedStatus.Value &&
                        (
                            (!filter.CommencedStart.HasValue && !filter.CommencedEnd.HasValue && a.ApprovedOn >= DateTime.UtcNow.AddMonths(-1))
                            || (
                                    (!filter.CommencedStart.HasValue || a.CommencedOn >= filter.CommencedStart) &&
                                    (!filter.CommencedEnd.HasValue || a.CommencedOn <= filter.CommencedEnd)
                                )
                            )
                        )
                    ))
             .Where(a => requiresQa.Contains(a.Type));

        Query
            .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
            .ThenByDescending(a => a.ApprovedOn)
            .ThenBy(a => a.LastModified);
    }
}