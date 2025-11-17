using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.PaymentProcessor.IntegrationEvents.Events;

public record PaymentProcessedIntegrationEvent(string ParticipantId) : IntegrationEvent;
