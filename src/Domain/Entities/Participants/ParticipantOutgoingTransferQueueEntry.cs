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
        DateTime moveOccured,
        string? previousOwnerId=null,
        string? previousTenantId = null
        )
    : base(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured) {
        PreviousOwnerId = previousOwnerId;
        PreviousTenantId = previousTenantId;
    }

    public static ParticipantOutgoingTransferQueueEntry Create(
        string participantId,
        Location fromLocation,
        Location toLocation,
        Contract? fromContract,
        Contract? toContract,
        DateTime moveOccured,
        string? previousOwnerId = null,
        string? previousTenantId = null
        ) => new(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured, previousOwnerId, previousTenantId);
    
    /// <summary>
    /// Represents the replaced status of an outgoing transfer. 
    /// A transfer gets replaced when another transfer takes place from the same contract.
    /// </summary>
    public bool IsReplaced { get; private set; }

    /// <summary>
    /// Represents the previous owner  who owned participant.
    /// </summary>
    public string? PreviousOwnerId { get; private set; }

    /// <summary>
    /// Represents the previous tenant who owned participant.
    /// </summary>
    public string? PreviousTenantId { get; private set; }   

    public ParticipantOutgoingTransferQueueEntry MarkAsReplaced()
    {
        IsReplaced = true;
        return this;
    }
}