using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;
using Humanizer;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public class ParticipantTransitionedEventHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork) : TimelineNotificationHandler<ParticipantTransitionedDomainEvent>(currentUserService, unitOfWork)
{
    protected override string GetLine1(ParticipantTransitionedDomainEvent notification) => $"Enrolment transitioned to {notification.To.Name.Humanize()}";
    protected override TimelineEventType GetEventType() => TimelineEventType.Enrolment;
    protected override string GetParticipantId(ParticipantTransitionedDomainEvent notification) => notification.Item.Id;
}
