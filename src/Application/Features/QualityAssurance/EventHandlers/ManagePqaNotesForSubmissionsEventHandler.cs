using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class ManagePqaNotesForSubmissionsEventHandler : INotificationHandler<EnrolmentQueueEntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentQueueEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry is EnrolmentPqaQueueEntry { IsAccepted: false, Notes.Count: > 0 } returned)
        {
            // if the PQA has returned the submission, add the notes to the generic notes for now 
            // so the caseworker has visibility
            var n = returned.Notes.First();
            returned.Participant!.AddNote(new Note()
            {
                Message = n.Message,
                TenantId = notification.Entry!.Participant!.Owner!.TenantId!
            });
        }
        
        return Task.CompletedTask;
    }
}
