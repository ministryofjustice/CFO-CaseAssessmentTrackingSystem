using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Bios.IntegrationEvents;

public record BioSubmittedIntegrationEvent(Guid BioId, DateTime OccurredOn)
    : IntegrationEvent(OccurredOn)
{
}