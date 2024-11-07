using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantOutgoingTransferQueueEntry : ParticipantTransferQueueEntry
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantOutgoingTransferQueueEntry()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ParticipantOutgoingTransferQueueEntry(
        string participantId,
        Location fromLocation,
        Location toLocation,
        Contract? fromContract,
        Contract? toContract,
        DateTime moveOccured)
    : base(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured) { }

    public static ParticipantOutgoingTransferQueueEntry Create(
        string participantId,
        Location fromLocation,
        Location toLocation,
        Contract? fromContract,
        Contract? toContract,
        DateTime moveOccured) => new(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured);
}
