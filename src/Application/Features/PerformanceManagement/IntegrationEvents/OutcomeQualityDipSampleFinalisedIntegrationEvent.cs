using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;

public record OutcomeQualityDipSampleFinalisedIntegrationEvent(Guid DipSampleId, string UserId, DateTime OccurredOn)
    : IntegrationEvent(OccurredOn);