using Cfo.Cats.Application.Features.QualityAssurance.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events.QA.Enrolments;

namespace Cfo.Cats.Application.Features.QualityAssurance.EventHandlers;

public class PublishEnrolmentApprovedAtQaEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<EnrolmentEscalationEntryCompletedDomainEvent>, 
        INotificationHandler<EnrolmentQa2EntryCompletedDomainEvent>

{
    public async Task Handle(EnrolmentEscalationEntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            await Publish(notification.Entry.ParticipantId, notification.DateOccurred.DateTime);
        }
    }
    
    public async Task Handle(EnrolmentQa2EntryCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Entry.IsAccepted)
        {
            await Publish(notification.Entry.ParticipantId, notification.DateOccurred.DateTime);
        }
    }
    private Task Publish(string participantId, DateTime dateOccurred)
    {
        return unitOfWork.DbContext.InsertOutboxMessage(new EnrolmentApprovedAtQaIntegrationEvent(participantId, dateOccurred));
    }
}
