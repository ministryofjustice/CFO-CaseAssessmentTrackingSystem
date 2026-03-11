using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class PathwayPlanReviewUpdatedDomainEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<PathwayPlanReviewUpdatedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.PathwayPlan;

    protected override string GetLine1(PathwayPlanReviewUpdatedDomainEvent notification) => "Pathway Plan Review updated.";

    protected override string GetParticipantId(PathwayPlanReviewUpdatedDomainEvent notification) => notification.Item.ParticipantId;
}
