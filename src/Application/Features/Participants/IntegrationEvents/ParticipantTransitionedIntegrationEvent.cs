using Cfo.Cats.Application.Common.IntegrationEvents;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public record ParticipantTransitionedIntegrationEvent(string ParticipantId, string From, string To) : IntegrationEvent;