using Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.PerformanceManagement.EventHandlers;

public class PublishOutcomeDipSampleFinalisingEvent(IUnitOfWork unitOfWork) : INotificationHandler<OutcomeQualityDipSampleFinalisedDomainEvent>
{
    public Task Handle(OutcomeQualityDipSampleFinalisedDomainEvent notification, CancellationToken cancellationToken)
       => unitOfWork.DbContext.InsertOutboxMessage(new OutcomeQualityDipSampleFinalisedIntegrationEvent(notification.DipSampleId, notification.UserId, notification.OccurredOn));
}