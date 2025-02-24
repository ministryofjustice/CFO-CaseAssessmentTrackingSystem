using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

internal class Qa2EscalatedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<EnrolmentQa2EntryEscalatedDomainEvent>
{
    public async Task Handle(EnrolmentQa2EntryEscalatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var entry = EnrolmentEscalationQueueEntry.Create(notification.Entry.ParticipantId);
        entry.TenantId = notification.Entry!.Participant!.Owner!.TenantId!;
        entry.SupportWorkerId = notification.Entry!.Participant!.Owner.Id!;
        await unitOfWork.DbContext.EnrolmentEscalationQueue.AddAsync(entry, cancellationToken);
    }
}