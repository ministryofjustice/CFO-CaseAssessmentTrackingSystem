using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantNameChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantNameChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantNameChangedDomainEvent notification) => "Participant name updated";

    protected override string? GetLine2(ParticipantNameChangedDomainEvent notification) => $"To {notification.To}";
    protected override string? GetLine3(ParticipantNameChangedDomainEvent notification) => $"From {notification.From}";

    protected override string GetParticipantId(ParticipantNameChangedDomainEvent notification) => notification.Item.Id;
}
