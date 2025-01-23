namespace Cfo.Cats.Application.Features.PRI.Specifications;

//Need changing for PRI when created
public class PRIAdvancedSpecification : Specification<Domain.Entities.PRIs.PRI>
{
    public PRIAdvancedSpecification(PRIAdvancedFilter filter)
    {
        //Query.Where(
        //    p => p.AssignedTo == filter.CurrentUser!.UserId
        //);

        //need changing for PRI
        //Query.Where(
        //c => c.EventType == (filter.ListView == TimelineTrailListView.Participant ? TimelineEventType.Participant :
        //    filter.ListView == TimelineTrailListView.Enrolment ? TimelineEventType.Enrolment :
        //    filter.ListView == TimelineTrailListView.Consent ? TimelineEventType.Consent :
        //    filter.ListView == TimelineTrailListView.Assessment ? TimelineEventType.Assessment : TimelineEventType.Participant),
        //    filter.ListView != TimelineTrailListView.All
        //);

        Query.OrderByDescending(t => t.Created);
    }
}