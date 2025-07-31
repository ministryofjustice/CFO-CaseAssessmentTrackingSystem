namespace Cfo.Cats.Application.Features.PerformanceManagement.IntegrationEvents;

public record OutcomeQualityDipSampleCompletedIntegrationEvent(string ReviewBy, Guid DipSampleId, DateTime OccurredOn);