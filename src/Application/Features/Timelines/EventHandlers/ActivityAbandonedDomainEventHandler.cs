using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ActivityAbandonedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<ActivityAbandonedDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(ActivityAbandonedDomainEvent notification) => "Activity abandoned.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Activity;
    protected override string GetParticipantId(ActivityAbandonedDomainEvent notification) 
        => notification.Item.ParticipantId;
}