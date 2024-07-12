using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class AssessmentCreatedDomainEventHandler(ICurrentUserService currentUserService, IApplicationDbContext context) : TimelineNotificationHandler<AssessmentCreatedDomainEvent>(currentUserService, context)
{
    protected override string GetLine1(AssessmentCreatedDomainEvent notification) => "Assessment created.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Assessment;
    protected override string GetParticipantId(AssessmentCreatedDomainEvent notification) 
        => notification.Entity.ParticipantId;
}

