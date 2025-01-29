namespace Cfo.Cats.Application.Features.PRIs.IntegrationEvents;

public record PRIAssignedIntegrationEvent(Guid PRIId, DateTime OccurredOn);
