namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public record ParticipantTransitionedIntegrationEvent(string ParticipantId, string From, string To, DateTime OccuredOn);