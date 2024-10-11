using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantNationalityChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantNationalityChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantNationalityChangedDomainEvent notification) => "Participant nationality updated";

    protected override string? GetLine2(ParticipantNationalityChangedDomainEvent notification) => $"To {notification.To}";
    protected override string? GetLine3(ParticipantNationalityChangedDomainEvent notification) => $"From {notification.From}";

    protected override string GetParticipantId(ParticipantNationalityChangedDomainEvent notification) => notification.Item.Id;
}
