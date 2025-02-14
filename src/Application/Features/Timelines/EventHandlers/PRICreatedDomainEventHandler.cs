using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class PRICreatedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<PRICreatedDomainEvent>(currentUserService, unitOfWork)
{    
    protected override string GetLine1(PRICreatedDomainEvent notification) => "PRI created.";
    protected override TimelineEventType GetEventType() => TimelineEventType.PRI;
    protected override string GetParticipantId(PRICreatedDomainEvent notification)
        => notification.Entity.ParticipantId;
}