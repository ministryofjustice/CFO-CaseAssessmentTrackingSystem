using AutoMapper.QueryableExtensions.Impl;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Timelines.Specifications;

public class TimelineAdvancedSpecification : Specification<Timeline>
{
    public TimelineAdvancedSpecification(TimelineAdvancedFilter filter)
    {
        Query.Where(
            p => p.ParticipantId == filter.ParticipantId
        );

        Query.Where(
        c => c.EventType == (filter.ListView == TimelineTrailListView.Participant ? TimelineEventType.Participant :
            filter.ListView == TimelineTrailListView.Enrolment ? TimelineEventType.Enrolment :
            filter.ListView == TimelineTrailListView.Consent ? TimelineEventType.Consent :
            filter.ListView == TimelineTrailListView.Assessment ? TimelineEventType.Assessment : TimelineEventType.Participant),
            filter.ListView != TimelineTrailListView.All
        );

        Query.OrderByDescending(t => t.Created);

    }
}
