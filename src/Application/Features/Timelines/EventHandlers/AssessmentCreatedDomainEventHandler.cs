using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class AssessmentCreatedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<AssessmentCreatedDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(AssessmentCreatedDomainEvent notification) => "Assessment created.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Assessment;
    protected override string GetParticipantId(AssessmentCreatedDomainEvent notification) 
        => notification.Entity.ParticipantId;
}

