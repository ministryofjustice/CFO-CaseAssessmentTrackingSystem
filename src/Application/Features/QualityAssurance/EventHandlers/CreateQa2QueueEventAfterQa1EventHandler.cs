using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class CreateQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<EnrolmentQa1EntryCompletedDomainEvent>
{
    public async Task Handle(EnrolmentQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var entry = EnrolmentQa2QueueEntry.Create(notification.Entry.ParticipantId);
        entry.TenantId = notification.Entry!.Participant!.Owner!.TenantId!;
        await unitOfWork.DbContext.EnrolmentQa2Queue.AddAsync(entry, cancellationToken);
    }
}