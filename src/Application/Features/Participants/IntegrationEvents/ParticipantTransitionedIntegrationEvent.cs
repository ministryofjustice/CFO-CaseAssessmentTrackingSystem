using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public sealed record ParticipantTransitionedIntegrationEvent(string ParticipantId, string From, string To, DateTime OccuredOn) : IntegrationEvent;