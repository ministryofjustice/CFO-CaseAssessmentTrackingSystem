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

        Query.Where(te => te.EventType == TimelineEventType.Participant, filter.ListView == TimelineTrailListView.Participant);
        Query.Where(te => te.EventType == TimelineEventType.Enrolment, filter.ListView == TimelineTrailListView.Enrolment);
        Query.Where(te => te.EventType == TimelineEventType.Consent, filter.ListView == TimelineTrailListView.Consent);
        Query.Where(te => te.EventType == TimelineEventType.Assessment, filter.ListView == TimelineTrailListView.Assessment);
        Query.Where(te => te.EventType == TimelineEventType.PathwayPlan, filter.ListView == TimelineTrailListView.PathwayPlan);
        Query.Where(te => te.EventType == TimelineEventType.Bio, filter.ListView == TimelineTrailListView.Bio);
        Query.Where(te => te.EventType == TimelineEventType.Dms, filter.ListView == TimelineTrailListView.Dms);
        Query.Where(te => te.EventType == TimelineEventType.PRI, filter.ListView == TimelineTrailListView.PRI);
        Query.Where(te => te.EventType == TimelineEventType.Activity, filter.ListView == TimelineTrailListView.Activity);
        
        Query.OrderByDescending(t => t.Created);

    }
}
