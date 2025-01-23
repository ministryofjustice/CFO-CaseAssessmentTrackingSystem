using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;

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
    public DateOnly? ActualReleaseDate { get; private set; }
    public DateTime? AcceptedOn { get; private set; }
    public int ExpectedReleaseRegionId { get; private set; }
    public virtual Location ExpectedReleaseRegion { get; private set; }
    public string? AssignedTo { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateOnly MeetingAttendedOn { get; private set; }
    public bool CustodyAttendedInPerson => ReasonCustodyDidNotAttendInPerson is null;
    public bool CommunityAttendedInPerson => ReasonCommunityDidNotAttendInPerson is null;
    public bool ParticipantAttendedInPerson => ReasonParticipantDidNotAttendInPerson is null;
    public string? ReasonCustodyDidNotAttendInPerson { get; private set; }
    public string? ReasonCommunityDidNotAttendInPerson { get; private set; }
    public string? ReasonParticipantDidNotAttendInPerson { get; private set; }

    ////Created User
    //public virtual ApplicationUser? CustodyWorker { get; set; }

    ////Assigned To
    //public virtual ApplicationUser? SupportWorker { get; set; }


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
        // AddDomainEvent(new PRIAssignedDomainEvent(this));
        return this;
    }

    public PRI Complete()
    {
        IsCompleted = true;
        //AddDomainEvent(new PRICompletedDomainEvent(this));
        return this;
    }

    public PRI Accept(DateTime acceptedOn)
    {
        AcceptedOn = acceptedOn;
        return this;
    }

    public PRI SetActualReleaseDate(DateOnly actualReleaseDate)
    {
        ActualReleaseDate = actualReleaseDate;
        return this;
    }

    public PRI WithMeeting(
        DateOnly attendedOn, 
        string? reasonCustodyDidNotAttendInPerson = null,
        string? reasonCommunityDidNotAttendInPerson = null,
        string? reasonParticipantDidNotAttendInPerson = null)
    {
        MeetingAttendedOn = attendedOn;
        ReasonCommunityDidNotAttendInPerson = reasonCommunityDidNotAttendInPerson;
        ReasonCustodyDidNotAttendInPerson = reasonCustodyDidNotAttendInPerson;
        ReasonParticipantDidNotAttendInPerson = reasonParticipantDidNotAttendInPerson;
        return this;
    }

}
