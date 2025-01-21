using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.PRIs;

public class PRI : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PRI()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string ParticipantId { get; private set; }
    public DateOnly ExpectedReleaseDate { get; private set; }
    public int ExpectedReleaseRegionId { get; private set; }
    public virtual Location ExpectedReleaseRegion { get; private set; }
    public string? AssignedTo { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateOnly MeetingAttendedOn { get; private set; }
    public bool MeetingAttendedInPerson { get; private set; }
    public string? MeetingNotAttendedInPersonJustification { get; private set; }

    public static PRI Create(string participantId, DateOnly expectedReleaseDate, int expectedReleaseRegionId)
    {
        var pri = new PRI()
        {
            ParticipantId = participantId,
            ExpectedReleaseDate = expectedReleaseDate,
            ExpectedReleaseRegionId = expectedReleaseRegionId
        };

        pri.AddDomainEvent(new PRICreatedDomainEvent(pri));

        return pri;
    }

    public PRI AssignTo(string? to)
    {
        AssignedTo = to;
        return this;
    }

    public PRI Complete()
    {
        IsCompleted = true;
        //AddDomainEvent(new PRICompletedDomainEvent(this));
        return this;
    }

    public PRI WithMeeting(DateOnly attendedOn, bool attendedInPerson, string? notAttendedInPersonJustification = null)
    {
        MeetingAttendedOn = attendedOn;
        MeetingAttendedInPerson = attendedInPerson;
        MeetingNotAttendedInPersonJustification = notAttendedInPersonJustification;
        return this;
    }

}
