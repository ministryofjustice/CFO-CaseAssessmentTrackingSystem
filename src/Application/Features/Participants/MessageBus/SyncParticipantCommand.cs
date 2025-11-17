using Cfo.Cats.Application.Common.MessageBus;
using Cfo.Cats.EventBus.Events;

namespace Cfo.Cats.Application.Features.Participants.MessageBus;

public record SyncParticipantCommand(string ParticipantId) : IntegrationEvent, ICommand;