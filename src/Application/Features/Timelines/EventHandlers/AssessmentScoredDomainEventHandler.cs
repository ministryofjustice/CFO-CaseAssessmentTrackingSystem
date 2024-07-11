using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class AssessmentScoredEventHandler(ICurrentUserService currentUserService, IApplicationDbContext context) : TimelineNotificationHandler<AssessmentScoredDomainEvent>(currentUserService, context)
{
    protected override string GetLine1(AssessmentScoredDomainEvent notification) => "Assessment submitted and scored.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Assessment;
    protected override string GetParticipantId(AssessmentScoredDomainEvent notification) 
        => notification.Entity.ParticipantId;
}

