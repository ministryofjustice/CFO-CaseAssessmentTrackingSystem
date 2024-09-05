using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantRegistrationDetailsChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantRegistrationDetailsChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantRegistrationDetailsChangedDomainEvent notification) => "Participant Registration Details / MAPPA updated";

    protected override string GetParticipantId(ParticipantRegistrationDetailsChangedDomainEvent notification) => notification.Item.Id;
}
