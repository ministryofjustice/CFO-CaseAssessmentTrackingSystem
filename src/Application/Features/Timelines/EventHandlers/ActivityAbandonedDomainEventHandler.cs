using Cfo.Cats.Application.Features.Activities.IntegrationEvents;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ActivityAbandonedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) 
    : TimelineNotificationHandler<ActivityAbandonedIntegrationEvent>(currentUserService, unitOfWork)
{    
    protected override string GetLine1(ActivityAbandonedIntegrationEvent notification) => "Activity abandoned.";
    protected override TimelineEventType GetEventType() => TimelineEventType.Activity;
    protected override string GetParticipantId(ActivityAbandonedIntegrationEvent notification) 
        => notification.Item.ParticipantId;
}