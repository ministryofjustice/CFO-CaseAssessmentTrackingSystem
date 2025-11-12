using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ActivityAbandonedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) 
    : TimelineNotificationHandler<ActivityTransitionedDomainEvent>(currentUserService, unitOfWork)
{   
    protected override string GetLine1(ActivityTransitionedDomainEvent notification) => "Activity abandoned.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Activity;
    
    protected override string GetParticipantId(ActivityTransitionedDomainEvent notification) 
        => notification.Item.ParticipantId;
}

