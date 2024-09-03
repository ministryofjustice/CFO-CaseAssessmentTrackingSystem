using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantMappaChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantMappaChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantMappaChangedDomainEvent notification) => $"Mappa (Category, Level) changed";

    protected override string? GetLine2(ParticipantMappaChangedDomainEvent notification) => $"To {notification.ToCategory.Name}, {notification.ToLevel.Name}";
    protected override string? GetLine3(ParticipantMappaChangedDomainEvent notification) => $"From {notification.FromCategory.Name}, {notification.FromLevel.Name}";

    protected override string GetParticipantId(ParticipantMappaChangedDomainEvent notification) => notification.Item.Id;
}
