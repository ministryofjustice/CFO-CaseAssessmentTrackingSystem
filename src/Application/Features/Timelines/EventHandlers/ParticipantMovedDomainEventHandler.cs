using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantMovedDomainEventHandler(
    ICurrentUserService currentUserService, 
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantMovedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.PathwayPlan;

    protected override string GetLine1(ParticipantMovedDomainEvent notification) => "Participant moved locations";

    protected override string? GetLine2(ParticipantMovedDomainEvent notification) => $"To {notification.To.Name}";
    protected override string? GetLine3(ParticipantMovedDomainEvent notification) => $"From {notification.From.Name}";

    protected override string GetParticipantId(ParticipantMovedDomainEvent notification) => notification.Item.Id;
}
