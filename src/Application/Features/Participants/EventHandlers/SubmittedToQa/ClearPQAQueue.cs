using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers.SubmittedToQa;




public class ClearPQAQueue(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantTransitionedDomainEvent>
{

    public async Task Handle(ParticipantTransitionedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        if (notification.To == EnrolmentStatus.SubmittedToAuthorityStatus)
        {
            // remove all PQA
            var events = unitOfWork.DbContext.EnrolmentPqaQueue
                .Where(e => e.ParticipantId == notification.Item.Id)
                .Where(e => e.IsCompleted == false);

            foreach (var e in events)
            {
                // should only be one.
                e.Complete();
            }
        }
    }
}