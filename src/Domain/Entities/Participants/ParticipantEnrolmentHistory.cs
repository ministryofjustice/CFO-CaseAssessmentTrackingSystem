using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;

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

    public static ParticipantEnrolmentHistory Create(string participantId, EnrolmentStatus enrolmentStatus, string? reason, string? additionalInformation)
        => new()
        {
            ParticipantId = participantId,
            EnrolmentStatus = enrolmentStatus,
            Reason= reason,
            AdditionalInformation = additionalInformation,
            Created = DateTime.UtcNow
        };
}