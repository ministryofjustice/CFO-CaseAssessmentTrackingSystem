using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantOwnershipHistory : BaseAuditableEntity<int>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantOwnershipHistory()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ParticipantOwnershipHistory(string participantId, string? ownerId, string? tenantId, DateTime from)
    {
        ParticipantId = participantId;
        TenantId = tenantId;
        OwnerId = ownerId;
        From = from;
    }

    public static ParticipantOwnershipHistory Create(string participantId, string? ownerId, string? tenantId, DateTime from) => new(participantId, ownerId, tenantId, from);

    public ParticipantOwnershipHistory SetTo(DateTime to)
    {
        To = to;
        return this;
    }

    public string? TenantId { get; private set; }
    public string ParticipantId { get; private set; }
    public string? OwnerId { get; private set; }
    public DateTime From { get; private set; }
    public DateTime? To { get; private set; }
}
