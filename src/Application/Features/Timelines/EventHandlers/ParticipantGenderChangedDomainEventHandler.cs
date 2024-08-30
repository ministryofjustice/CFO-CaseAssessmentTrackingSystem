using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantGenderChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantGenderChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantGenderChangedDomainEvent notification) => "Participant gender updated";

    protected override string? GetLine2(ParticipantGenderChangedDomainEvent notification) => $"To {notification.To}";
    protected override string? GetLine3(ParticipantGenderChangedDomainEvent notification) => $"From {notification.From}";

    protected override string GetParticipantId(ParticipantGenderChangedDomainEvent notification) => notification.Item.Id;
}
