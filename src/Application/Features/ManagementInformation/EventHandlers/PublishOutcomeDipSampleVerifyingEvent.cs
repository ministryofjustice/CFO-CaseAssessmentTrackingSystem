using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishOutcomeDipSampleVerifyingEvent(IUnitOfWork unitOfWork) : INotificationHandler<OutcomeQualityDipSampleVerifyingDomainEvent>
{
    public async Task Handle(OutcomeQualityDipSampleVerifyingDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.InsertOutboxMessage(new OutcomeQualityDipSampleVerifyingIntegrationEvent(notification.DipSampleId, notification.UserId, notification.OccurredOn));
    }
}