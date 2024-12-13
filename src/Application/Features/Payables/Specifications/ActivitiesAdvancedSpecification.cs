using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Specifications;

public class ActivitiesAdvancedSpecification : Specification<Activity>
{
    public ActivitiesAdvancedSpecification(ActivitiesAdvancedFilter filter)
    {
        Query.Where(a => a.ParticipantId == filter.ParticipantId)
             .Where(a => a.TaskId == filter.TaskId, filter.TaskId is not null)
             .Where(a => a.ObjectiveId == filter.ObjectiveId, filter.ObjectiveId is not null)
             .Where(a => a.TookPlaceAtLocation.Id == filter.Location!.Id, filter.Location is not null)
             .Where(a => filter.IncludeTypes!.Contains(a.Type), filter.IncludeTypes is { Count: > 0 })
             .Where(a => a.Completed >= filter.CompletedStart, filter.CompletedStart is not null)
             .Where(a => a.Completed <= filter.CompletedEnd, filter.CompletedEnd is not null)
             .Where(a => a.Status == filter.Status, filter.Status is not null);
    }
}