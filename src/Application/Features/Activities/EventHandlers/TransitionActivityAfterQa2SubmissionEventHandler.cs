using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers
{
    public class TransitionActivityAfterQa2SubmissionEventHandler
    : INotificationHandler<ActivityQa2EntryCompletedDomainEvent>
    {
        public Task Handle(ActivityQa2EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Entry.IsAccepted)
            {
                notification.Entry
                    .Activity!
                    .TransitionTo(ActivityStatus.ApprovedStatus);
            }
            else
            {
                notification.Entry
                    .Activity!
                    .TransitionTo(ActivityStatus.SubmittedToProviderStatus);
            }

            return Task.CompletedTask;
        }
    }
}