using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class BioCreatedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<BioCreatedDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(BioCreatedDomainEvent notification) => "Bio created";
    protected override TimelineEventType GetEventType() => TimelineEventType.Bio;
    protected override string GetParticipantId(BioCreatedDomainEvent notification)
        => notification.Entity.ParticipantId;
}
