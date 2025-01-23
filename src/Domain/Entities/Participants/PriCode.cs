using Cfo.Cats.Domain.Common.Entities;

namespace Cfo.Cats.Domain.Entities.Participants;

public class PriCode : BaseAuditableEntity<int>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private PriCode()
    {
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private PriCode(string participantId, string createdBy)
    {
        ParticipantId = participantId;
        CreatedBy = createdBy;
        Created = DateTime.UtcNow;
    }

    public string ParticipantId { get; set; }
    public int Code { get; set; }

    public static PriCode Create(string participantId, string createdBy)
    {
        PriCode priCode = new(participantId, createdBy);
        // AddDomainEvent(new PRICodeGeneratedDomainEvent());
        return priCode.GenerateCode();
    }

    PriCode GenerateCode()
    {
        Code = new Random().Next(100000, 999999);
        return this;
    }
}
