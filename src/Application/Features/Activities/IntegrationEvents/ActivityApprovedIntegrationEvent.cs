using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Activities.IntegrationEvents;

public record ActivityApprovedIntegrationEvent(Guid ActivitiyId, DateTime OccurredOn) 
    : IntegrationEvent(OccurredOn);
