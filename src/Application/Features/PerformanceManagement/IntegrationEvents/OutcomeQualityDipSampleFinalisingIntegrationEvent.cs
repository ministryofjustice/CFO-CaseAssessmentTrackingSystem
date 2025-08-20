namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;

public record OutcomeQualityDipSampleFinalisingIntegrationEvent(Guid DipSampleId, string UserId, DateTime OccurredOn);