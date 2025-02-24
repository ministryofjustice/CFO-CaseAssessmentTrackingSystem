using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class CreateQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<EnrolmentQa1EntryCompletedDomainEvent>
{
    public async Task Handle(EnrolmentQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var qa2 = EnrolmentQa2QueueEntry.Create(notification.Entry.ParticipantId);

        // get the most recent PQA entry
        var pqa = await unitOfWork
            .DbContext.EnrolmentPqaQueue
            .AsNoTracking()
            .Where(q => q.ParticipantId == notification.Entry.ParticipantId)
            .OrderByDescending(q => q.Created)
            .Take(1)
            .Select(q => new
            {
                q.TenantId,
                q.SupportWorkerId,
                q.ConsentDate
            })
            .FirstAsync(cancellationToken);

        qa2.TenantId = pqa.TenantId;
        qa2.SupportWorkerId = pqa.SupportWorkerId;
        qa2.ConsentDate = pqa.ConsentDate;

        await unitOfWork.DbContext.EnrolmentQa2Queue.AddAsync(qa2, cancellationToken);
    }
}