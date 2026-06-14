namespace Cfo.Cats.Application.Features.Participants.MessageBus;

public record SyncParticipantCommand(string ParticipantId) : Cfo.Cats.Application.Common.MessageBus.ICommand;