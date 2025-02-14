using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events.QA.Enrolments;
using Cfo.Cats.Domain.ValueObjects;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class CreateNoteForPqaReturnEventHandler : INotificationHandler<EnrolmentPqaEntryCompletedDomainEvent>
{
    public Task Handle(EnrolmentPqaEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry is { IsAccepted: false, Notes.Count: > 0 } returned)
        {
            // if the PQA has returned the submission, add the notes to the generic notes for now 
            // so the caseworker has visibility
            var n = returned.Notes.First();
            returned.Participant!.AddNote(new Note
            {
                Message = n.Message
            });
        }

        return Task.CompletedTask;
    }
}