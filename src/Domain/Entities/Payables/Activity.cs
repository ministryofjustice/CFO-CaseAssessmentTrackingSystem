using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public abstract class Activity : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Activity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected Activity(ActivityDefinition definition, string participantId, Location location, Contract contract, string? additionalInformation, DateTime completed, string completedBy)
    {
        Definition = definition;
        ParticipantId = participantId;
        Location = location;
        Contract = contract;
        AdditionalInformation = additionalInformation;
        Completed = completed;
        CompletedBy = completedBy;
        Status = ActivityStatus.Submitted;

        AddDomainEvent(new ActivityCreatedDomainEvent(this));
    }

    public ActivityDefinition Definition { get; protected set; }
    public string ParticipantId { get; protected set; }
    public virtual Location Location { get; protected set; }
    public virtual Contract Contract { get; protected set; }
    public string? AdditionalInformation { get; protected set; }
    public DateTime Completed { get; protected set; }
    public string CompletedBy { get; protected set; }
    public ActivityStatus Status { get; protected set; }

    public Activity TransitionTo(ActivityStatus to)
    {
        if(Status != to)
        {
            AddDomainEvent(new ActivityTransitionedDomainEvent(this, Status, to));
            Status = to;
        }

        return this;
    }

    public bool RequiresQa => Definition.CheckType == CheckType.QA;

}