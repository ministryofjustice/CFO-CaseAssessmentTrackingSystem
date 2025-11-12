namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public record ActivityAbandonedIntegrationEvent(Guid Id, DateTime OccurredOn);
