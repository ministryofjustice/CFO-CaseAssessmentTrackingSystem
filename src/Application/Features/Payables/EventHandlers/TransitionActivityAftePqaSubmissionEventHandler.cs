using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    public class TransitionActivityAftePqaSubmissionEventHandler
      : INotificationHandler<ActivityPqaEntryCompletedDomainEvent>
    {
        public Task Handle(ActivityPqaEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Entry.IsAccepted)
            {
                notification.Entry
                    .Activity!
                    .TransitionTo(ActivityStatus.SubmittedToAuthorityStatus);
            }
            else
            {
                notification.Entry
                    .Activity!
                    .TransitionTo(ActivityStatus.PendingStatus);
            }

            return Task.CompletedTask;
        }
    }
}
