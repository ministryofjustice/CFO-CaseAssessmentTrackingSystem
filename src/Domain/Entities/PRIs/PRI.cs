using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
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
    public DateOnly? ActualReleaseDate { get; private set; }
    public DateTime? AcceptedOn { get; private set; }
    public int ExpectedReleaseRegionId { get; private set; }
    public virtual Location ExpectedReleaseRegion { get; private set; }
    public string? AssignedTo { get; private set; }    
    public DateOnly MeetingAttendedOn { get; private set; }
    public bool CustodyAttendedInPerson => ReasonCustodyDidNotAttendInPerson is null;
    public bool CommunityAttendedInPerson => ReasonCommunityDidNotAttendInPerson is null;
    public bool ParticipantAttendedInPerson => ReasonParticipantDidNotAttendInPerson is null;
    public string? ReasonCustodyDidNotAttendInPerson { get; private set; }
    public string? ReasonCommunityDidNotAttendInPerson { get; private set; }
    public string? ReasonParticipantDidNotAttendInPerson { get; private set; }
    public string? PostReleaseCommunitySupportInformation { get; private set; }


    public virtual Participant? Participant { get; private set; }

    public int CustodyLocationId { get; private set; }

    public virtual Location CustodyLocation { get; private set; }
    public Guid ObjectiveId { get; private set; }

    /// <summary>
    /// Status of the  Pri
    /// </summary>
    public PriStatus Status { get; private set; }

    /// <summary>
    /// The justification for Abandoning Pri
    /// </summary>
    public string? AbandonJustification { get; private set; }

    /// <summary>
    /// The reason for Abandoning Pri
    /// </summary>
    public PriAbandonReason? AbandonReason { get; private set; }

    /// <summary>
    /// Who completed the Pri
    /// </summary>
    public string? CompletedBy { get; private set; }

    /// <summary>
    /// When the Pri was Completed
    /// </summary>
    public DateTime? CompletedOn { get; private set; }

    public static PRI Create(string participantId, DateOnly expectedReleaseDate, int expectedReleaseRegionId, string createdBy, int custodyLocationId)
    {
        var pri = new PRI()
        {
            ParticipantId = participantId,
            ExpectedReleaseDate = expectedReleaseDate,
            CreatedBy = createdBy,
            ExpectedReleaseRegionId = expectedReleaseRegionId,
            CustodyLocationId = custodyLocationId,
            Status = PriStatus.Created
        };

        pri.AddDomainEvent(new PRICreatedDomainEvent(pri));

        return pri;
    }

    public PRI AssignTo(string? to)
    {
        AssignedTo = to;
        AcceptedOn = DateTime.UtcNow;
        Status = PriStatus.Accepted;
        AddDomainEvent(new PRIAssignedDomainEvent(this, to));
        return this;
    }

    public PRI Complete(string? completedBy)
    {
        Status = PriStatus.Completed;
        CompletedOn = DateTime.UtcNow;
        CompletedBy = completedBy;
        AddDomainEvent(new PRICompletedDomainEvent(this));
        return this;
    }

    public PRI Abandon(PriAbandonReason abandonReason, string? abandonJustification, string abandonedBy)
    {
        Status = PriStatus.Abandoned;
        CompletedOn = DateTime.UtcNow;
        AbandonReason = abandonReason;
        AbandonJustification= abandonJustification;
        CompletedBy = abandonedBy;
        AddDomainEvent(new PRIAbandonedDomainEvent(this));
        return this;
    }

    public PRI SetActualReleaseDate(DateOnly actualReleaseDate)
    {
        ActualReleaseDate = actualReleaseDate;
        return this;
    }

    public PRI SetObjective(Objective objective)
    {
        ObjectiveId = objective.Id;
        return this;
    }

    public PRI WithMeeting(
        DateOnly attendedOn, 
        string? reasonCustodyDidNotAttendInPerson = null,
        string? reasonCommunityDidNotAttendInPerson = null,
        string? reasonParticipantDidNotAttendInPerson = null,
        string? postReleaseCommunitySupportInformation = null)
    {
        MeetingAttendedOn = attendedOn;
        ReasonCommunityDidNotAttendInPerson = reasonCommunityDidNotAttendInPerson;
        ReasonCustodyDidNotAttendInPerson = reasonCustodyDidNotAttendInPerson;
        ReasonParticipantDidNotAttendInPerson = reasonParticipantDidNotAttendInPerson;
        PostReleaseCommunitySupportInformation = postReleaseCommunitySupportInformation;
        return this;
    }
}