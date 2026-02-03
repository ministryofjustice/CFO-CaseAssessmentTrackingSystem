namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public record ActivityTransitionedIntegrationEvent(Guid ActivityId, string From, string To, DateTime OccuredOn);