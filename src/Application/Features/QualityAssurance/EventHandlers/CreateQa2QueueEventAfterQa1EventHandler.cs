using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class CreateQa2QueueEventAfterQa1EventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<EnrolmentQa1EntryCompletedDomainEvent>
{
    public async Task Handle(EnrolmentQa1EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {

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

        var qa2 = new EnrolmentQa2QueueEntry(notification.Entry.ParticipantId, pqa.TenantId, pqa.SupportWorkerId, pqa.ConsentDate);
        
        await unitOfWork.DbContext.EnrolmentQa2Queue.AddAsync(qa2, cancellationToken);
    }
}