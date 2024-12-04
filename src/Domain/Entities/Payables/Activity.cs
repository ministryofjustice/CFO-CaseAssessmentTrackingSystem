using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Domain.Entities.Payables;

public abstract class Activity : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Activity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected Activity(ActivityDefinition definition, string participantId, string description, Location location, Contract contract, string? additionalInformation, DateTime completed, string completedBy)
    {
        Definition = definition;
        ParticipantId = participantId;
        Description = description;
        Location = location;
        Contract = contract;
        AdditionalInformation = additionalInformation;
        Completed = completed;
        CompletedBy = completedBy;
    }

    public ActivityDefinition Definition { get; protected set; }
    public string ParticipantId { get; protected set; }
    public string Description { get; protected set; }
    public virtual Location Location { get; protected set; }
    public virtual Contract Contract { get; protected set; }
    public string? AdditionalInformation { get; protected set; }
    public DateTime Completed { get; protected set; }
    public string CompletedBy { get; protected set; }
}
