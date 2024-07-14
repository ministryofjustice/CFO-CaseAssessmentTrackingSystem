using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class AssessmentScoredEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<AssessmentScoredDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(AssessmentScoredDomainEvent notification) => "Assessment submitted and scored.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Assessment;
    protected override string GetParticipantId(AssessmentScoredDomainEvent notification) 
        => notification.Entity.ParticipantId;
}

