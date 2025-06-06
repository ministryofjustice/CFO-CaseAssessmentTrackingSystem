﻿using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantIncomingTransferQueueEntry : ParticipantTransferQueueEntry
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantIncomingTransferQueueEntry()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ParticipantIncomingTransferQueueEntry(
        string participantId, 
        Location fromLocation, 
        Location toLocation, 
        Contract? fromContract, 
        Contract? toContract, 
        DateTime moveOccured)
    : base(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured) { }

    public static ParticipantIncomingTransferQueueEntry Create(
        string participantId, 
        Location fromLocation, 
        Location toLocation, 
        Contract? fromContract, 
        Contract? toContract, 
        DateTime moveOccured) => new(participantId, fromLocation, toLocation, fromContract, toContract, moveOccured);

    /// <summary>
    /// The completion status of the transfer.
    /// </summary>
    public bool Completed { get; private set; }

    public ParticipantIncomingTransferQueueEntry Complete()
    {
        Completed = true;
        //AddDomainEvent(new IncomingTransferCompletedDomainEvent());
        return this;
    }

}
