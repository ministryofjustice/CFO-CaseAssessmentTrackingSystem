using Cfo.Cats.Domain.Events.QA.Activity;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    public class TransitionParticipantAfterActivityPqaSubmissionEventHandler
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
                    .TransitionTo(ActivityStatus.CreatingStatus);
            }

            return Task.CompletedTask;
        }
    }
}