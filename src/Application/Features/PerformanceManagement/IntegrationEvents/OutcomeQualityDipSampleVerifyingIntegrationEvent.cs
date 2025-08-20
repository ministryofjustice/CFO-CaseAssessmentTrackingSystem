namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;

public record OutcomeQualityDipSampleVerifyingIntegrationEvent(Guid DipSampleId, string UserId, DateTime OccurredOn);
