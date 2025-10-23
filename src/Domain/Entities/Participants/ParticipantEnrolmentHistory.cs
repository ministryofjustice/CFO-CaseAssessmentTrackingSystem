using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ParticipantEnrolmentHistory : BaseAuditableEntity<int>
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ParticipantEnrolmentHistory()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string ParticipantId { get; private set; }
    public EnrolmentStatus EnrolmentStatus { get; private set; }
    public string? Reason { get; private set; }
    public string? AdditionalInformation { get; private set; }

    public DateTime From { get; private set; }

    /// <summary>
    /// The date and time the enrolment status lasts till. If null, this is the current status
    /// </summary>
    public DateTime? To { get; private set; }

    public virtual Participant? Participant { get; private set; }

    public static ParticipantEnrolmentHistory Create(string participantId, EnrolmentStatus enrolmentStatus, string? reason, string? additionalInformation)
    {
        var history = new ParticipantEnrolmentHistory()
        {
            ParticipantId = participantId,
            EnrolmentStatus = enrolmentStatus,
            Reason = reason,
            AdditionalInformation = additionalInformation,
            From = DateTime.UtcNow
        };

        history.AddDomainEvent(new ParticipantEnrolmentHistoryCreatedDomainEvent(history));
        return history;
    }

    public ParticipantEnrolmentHistory SetTo(DateTime to)
    {
        if(To is not null)
        {
            throw new ApplicationException("Cannot set the to date more than once.");
        }

        if(to < From)
        {
            throw new ArgumentException("To cannot be earlier that From");
        }

        To = to;
        return this;
    }
}