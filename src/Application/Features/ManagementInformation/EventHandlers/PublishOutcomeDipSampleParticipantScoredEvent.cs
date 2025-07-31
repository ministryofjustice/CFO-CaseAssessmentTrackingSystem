using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishOutcomeDipSampleParticipantScoredEvent(IUnitOfWork unitOfWork) : INotificationHandler<OutcomeQualityDipSampleParticipantScoredDomainEvent>
{
    public async Task Handle(OutcomeQualityDipSampleParticipantScoredDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.InsertOutboxMessage(new OutcomeQualityDipSampleCompletedIntegrationEvent(notification.ReviewBy, notification.DipSampleId, notification.DateOccurred.DateTime));
    }
}