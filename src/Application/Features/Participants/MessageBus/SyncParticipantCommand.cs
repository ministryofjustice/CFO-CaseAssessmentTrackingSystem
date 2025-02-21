using Cfo.Cats.Application.Common.MessageBus;

namespace Cfo.Cats.Application.Features.Participants.MessageBus;

public record SyncParticipantCommand(string ParticipantId) : ICommand;