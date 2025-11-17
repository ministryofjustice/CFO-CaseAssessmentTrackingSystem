using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ActivityAbandonedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) 
    : TimelineNotificationHandler<ActivityTransitionedDomainEvent>(currentUserService, unitOfWork)
{   
    protected override string GetLine1(ActivityTransitionedDomainEvent notification) => "Activity abandoned.";
      
    protected override string GetLine2(ActivityTransitionedDomainEvent notification)
    {
        var name = notification.Item.Definition.Name;
        return name.Substring(0, Math.Min(name.Length, 50));
    }
    
    protected override string GetLine3(ActivityTransitionedDomainEvent notification) => notification.Item.AbandonReason!.Name;

    protected override TimelineEventType GetEventType() => TimelineEventType.Activity;

    protected override string GetParticipantId(ActivityTransitionedDomainEvent notification) 
        => notification.Item.ParticipantId;

    public override async Task Handle(ActivityTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        if(notification.Item.Status == ActivityStatus.AbandonedStatus)
        {
            await base.Handle(notification, cancellationToken); 
        }
    }
}