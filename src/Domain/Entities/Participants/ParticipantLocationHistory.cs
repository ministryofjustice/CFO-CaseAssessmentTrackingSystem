using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantLocationHistory : BaseAuditableEntity<int>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantLocationHistory()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ParticipantLocationHistory(string participantId, int locationId, DateTime from)
    {
        ParticipantId = participantId;
        LocationId = locationId;
        From = from;
    }

    public static ParticipantLocationHistory Create(string participantId, int locationId, DateTime from) => new(participantId, locationId, from);

    public string ParticipantId { get; private set; }
    public int LocationId { get; private set; }
    public DateTime From { get; private set; }
}
