using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantDateOfBirthChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantDateOfBirthChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantDateOfBirthChangedDomainEvent notification) => "Participant date of birth updated";

    protected override string? GetLine2(ParticipantDateOfBirthChangedDomainEvent notification) => $"To {notification.To}";
    protected override string? GetLine3(ParticipantDateOfBirthChangedDomainEvent notification) => $"From {notification.From}";

    protected override string GetParticipantId(ParticipantDateOfBirthChangedDomainEvent notification) => notification.Item.Id;
}
