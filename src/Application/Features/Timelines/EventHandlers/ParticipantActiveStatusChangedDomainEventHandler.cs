using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantActiveStatusChangedDomainEventHandler(
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantActiveStatusChangedDomainEvent>(currentUserService, unitOfWork)
{
    protected override TimelineEventType GetEventType() => TimelineEventType.Dms;

    protected override string GetLine1(ParticipantActiveStatusChangedDomainEvent notification) => "Participant active status updated";

    protected override string? GetLine2(ParticipantActiveStatusChangedDomainEvent notification) => string.Format("To {0}", notification.To ? "Active" : "Inactive");
    protected override string? GetLine3(ParticipantActiveStatusChangedDomainEvent notification) => string.Format("From {0}", notification.From ? "Active" : "Inactive");

    protected override string GetParticipantId(ParticipantActiveStatusChangedDomainEvent notification) => notification.Item.Id;
}
