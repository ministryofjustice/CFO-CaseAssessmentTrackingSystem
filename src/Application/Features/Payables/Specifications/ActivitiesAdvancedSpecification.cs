using Cfo.Cats.Domain.Entities.Payables;

namespace Cfo.Cats.Application.Features.Payables.Specifications;

public class ActivitiesAdvancedSpecification : Specification<Activity>
{
    public ActivitiesAdvancedSpecification(ActivitiesAdvancedFilter filter)
    {
        Query.Where(a => a.ParticipantId == filter.ParticipantId)
             .Where(a => a.TaskId == filter.TaskId, filter.TaskId is not null)
             .Where(a => a.TookPlaceAtLocation.Id == filter.Location!.Id, filter.Location is not null)
             .Where(a => filter.IncludeTypes!.Contains(a.Type), filter.IncludeTypes is { Count: > 0 });
    }
}