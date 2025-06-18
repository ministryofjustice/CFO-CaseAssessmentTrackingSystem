namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public record ParticipantCreatedIntegrationEvent(string ParticipantId, DateTime OccurredOn);