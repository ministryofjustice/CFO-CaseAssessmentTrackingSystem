using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Participants.IntegrationEvents;

public sealed record ParticipantCreatedIntegrationEvent(string ParticipantId, string? PrimaryRecordKeyAtCreation, DateTime OccurredOn) : IntegrationEvent;