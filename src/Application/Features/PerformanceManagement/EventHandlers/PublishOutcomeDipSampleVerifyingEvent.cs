using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PerformanceManagement.EventHandlers;

public class PublishOutcomeDipSampleVerifyingEvent(IUnitOfWork unitOfWork) : INotificationHandler<OutcomeQualityDipSampleVerifyingDomainEvent>
{
    public Task Handle(OutcomeQualityDipSampleVerifyingDomainEvent notification, CancellationToken cancellationToken)
        => unitOfWork.DbContext.InsertOutboxMessage(new OutcomeQualityDipSampleVerifyingIntegrationEvent(notification.DipSampleId, notification.UserId, notification.OccurredOn));
}
