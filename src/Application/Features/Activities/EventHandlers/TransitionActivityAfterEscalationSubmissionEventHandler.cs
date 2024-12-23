using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers
{
    public class TransitionActivityAfterEscalationSubmissionEventHandler
        : INotificationHandler<ActivityEscalationEntryCompletedDomainEvent>
    {
        public Task Handle(ActivityEscalationEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
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