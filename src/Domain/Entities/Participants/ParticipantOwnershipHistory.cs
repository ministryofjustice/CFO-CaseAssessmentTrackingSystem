using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantOwnershipHistory : BaseAuditableEntity<int>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantOwnershipHistory()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ParticipantOwnershipHistory(string participantId, string? ownerId, string? tenantId, string? contractId, DateTime from)
    {
        ParticipantId = participantId;
        TenantId = tenantId;
        OwnerId = ownerId;
        ContractId = contractId;
        From = from;
    }

    public static ParticipantOwnershipHistory Create(string participantId, string? ownerId, string? tenantId, string? contractId, DateTime from) => new(participantId, ownerId, tenantId, contractId, from);

    public ParticipantOwnershipHistory SetTo(DateTime to)
    {
        To = to;
        return this;
    }

    /// <summary>
    /// The tenant id the owner is associated with. If there is no owner, this will be null.
    /// </summary>
    public string? TenantId { get; private set; }

    /// <summary>
    /// The participant id. 
    /// </summary>
    public string ParticipantId { get; private set; }

    /// <summary>
    /// The support worker who owns the participant. If there is no owner, this will be null.
    /// </summary>
    public string? OwnerId { get; private set; }

    /// <summary>
    /// The contract id that owns the participant. If there is no owner, this will be based on the participants location.
    /// If the participant is in an unknown location, this will be null.
    /// </summary>
    public string? ContractId { get; private set; }

    /// <summary>
    /// The date and time the ownership started.
    /// </summary>
    public DateTime From { get; private set; }

    /// <summary>
    /// The date and time the ownership ended. If the ownership is current, this will be null.
    /// </summary>
    public DateTime? To { get; private set; }
}
