using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Participants;

public abstract class ParticipantTransferQueueEntry : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected ParticipantTransferQueueEntry()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected ParticipantTransferQueueEntry(
        string participantId,
        Location fromLocation,
        Location toLocation,
        Contract? fromContract,
        Contract? toContract,
        DateTime moveOccured)
    {
        ParticipantId = participantId;
        FromContract = fromContract;
        ToContract = toContract;
        FromLocation = fromLocation;
        ToLocation = toLocation;
        MoveOccured = moveOccured;
        TransferType = TransferLocationType.DetermineFromLocationTypes(fromLocation, toLocation);
    }

    /// <summary>
    /// The date the movement occurred.
    /// </summary>
    public DateTime MoveOccured { get; private set; }

    /// <summary>
    /// The type of transfer (eg: Community to Custody).
    /// </summary>
    public TransferLocationType TransferType { get; private set; }

    /// <summary>
    /// The participant due to transfer.
    /// </summary>
    public string ParticipantId { get; private set; }

    /// <summary>
    /// The original (source) contract, may be null (out of contract).
    /// </summary>
    public Contract? FromContract { get; private set; }

    /// <summary>
    /// The destination contract, may be null (out of contract).
    /// </summary>
    public Contract? ToContract { get; private set; }

    /// <summary>
    /// The original (source) location, may be in custody or the community.
    /// </summary>
    public Location FromLocation { get; private set; }

    /// <summary>
    /// The destination location, may be in custody or the community.
    /// </summary>
    public Location ToLocation { get; private set; }

}
