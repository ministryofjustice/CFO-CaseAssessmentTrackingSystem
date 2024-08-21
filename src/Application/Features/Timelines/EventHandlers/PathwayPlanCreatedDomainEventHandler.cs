using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class PathwayPlanCreatedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<PathwayPlanCreatedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.PathwayPlan;

    protected override string GetLine1(PathwayPlanCreatedDomainEvent notification) => "Pathway Plan created.";

    protected override string GetParticipantId(PathwayPlanCreatedDomainEvent notification) => notification.Item.ParticipantId;
}
