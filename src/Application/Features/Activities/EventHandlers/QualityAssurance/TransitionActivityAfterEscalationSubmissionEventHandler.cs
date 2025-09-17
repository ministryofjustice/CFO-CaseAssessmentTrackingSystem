using Cfo.Cats.Domain.Events.QA.Payables;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers.QualityAssurance;

public class TransitionActivityAfterEscalationSubmissionEventHandler(ICurrentUserService currentUserService) 
    : INotificationHandler<ActivityEscalationEntryCompletedDomainEvent>
{
    public Task Handle(ActivityEscalationEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            notification.Entry.Activity!.Approve(currentUserService.UserId);
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