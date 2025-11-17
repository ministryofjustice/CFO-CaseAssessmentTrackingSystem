using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.PaymentProcessor.IntegrationEvents.Events;

public sealed record ParticipantTransitionedIntegrationEvent(string ParticipantId, string From, string To, DateTime OccuredOn) : IntegrationEvent;
