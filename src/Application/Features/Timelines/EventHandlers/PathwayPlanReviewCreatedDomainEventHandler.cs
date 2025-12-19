using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class PathwayPlanReviewAddedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<PathwayPlanReviewAddedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.PathwayPlan;

    protected override string GetLine1(PathwayPlanReviewAddedDomainEvent notification) => "Pathway Plan Review created.";

    protected override string GetParticipantId(PathwayPlanReviewAddedDomainEvent notification) => notification.Item.ParticipantId;
}
