using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Assessments.EventHandlers;

public class PublishAssessmentScoredDomainEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<AssessmentScoredDomainEvent>
{
    public async Task Handle(AssessmentScoredDomainEvent notification, CancellationToken cancellationToken)
    {
        var e = new AssessmentScoredIntegrationEvent(notification.Entity.Id, notification.Entity.ParticipantId, notification.Entity.Completed!.Value);
        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}