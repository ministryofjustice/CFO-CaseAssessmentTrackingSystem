using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantActiveStatusHistory : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ParticipantActiveStatusHistory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static ParticipantActiveStatusHistory Create(string participantId, bool from, bool to, DateOnly occurredOn) => new()
    {
        Id = Guid.CreateVersion7(),
        ParticipantId = participantId,
        From = from,
        To = to,
        OccurredOn = occurredOn
    };

    public bool From { get; private set; }
    public bool To { get; private set; }
    public DateOnly OccurredOn { get; private set; }
    public string ParticipantId { get; private set; }
}
