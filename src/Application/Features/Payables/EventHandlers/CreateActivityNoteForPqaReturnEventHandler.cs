using Cfo.Cats.Domain.Events.QA.Payables;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.Payables.EventHandlers
{
    public class CreateActivityNoteForPqaReturnEventHandler : INotificationHandler<ActivityPqaEntryCompletedDomainEvent>
    {
        public Task Handle(ActivityPqaEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
        {
            if (notification.Entry is { IsAccepted: false, Notes.Count: > 0 } returned)
            {
                // if the PQA has returned the activity, add the notes to the generic notes for now 
                // so the caseworker has visibility
                var n = returned.Notes.First();
                returned.Participant!.AddNote(new Note
                {
                    Message = n.Message,
                    TenantId = notification.Entry!.Participant!.Owner!.TenantId!
                });
            }

            return Task.CompletedTask;
        }
    }
}
