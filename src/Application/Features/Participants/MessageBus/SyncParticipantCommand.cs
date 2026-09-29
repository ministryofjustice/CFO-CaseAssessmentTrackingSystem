using Cfo.Cats.Application.Common.MessageBus;
using Cfo.Cats.Application.Outbox;

namespace Cfo.Cats.Application.Features.Participants.MessageBus;

public record SyncParticipantCommand(string ParticipantId) : IntegrationEvent(DateTime.UtcNow), IExternalCommand;