using Ardalis.SmartEnum;
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

    protected Activity(
        ActivityDefinition definition, 
        string participantId, 
        Location tookPlaceAtLocation, 
        Contract tookPlaceAtContract, 
        Location participantCurrentLocation,
        Contract? participantCurrentContract,
        EnrolmentStatus participantStatus,
        string? additionalInformation, 
        DateTime completed, 
        string tenantId)
    {
        Definition = definition;
        ParticipantId = participantId;
        TookPlaceAtLocation = tookPlaceAtLocation;
        TookPlaceAtContract = tookPlaceAtContract;
        ParticipantCurrentLocation = participantCurrentLocation;
        ParticipantCurrentContract = participantCurrentContract;
        ParticipantStatus = participantStatus;
        AdditionalInformation = additionalInformation;
        Completed = completed;
        TenantId = tenantId;
        Status = ActivityStatus.Submitted;

        AddDomainEvent(new ActivityCreatedDomainEvent(this));
    }

    public ActivityDefinition Definition { get; protected set; }
    public string ParticipantId { get; protected set; }
    public virtual Location TookPlaceAtLocation { get; protected set; }
    public virtual Contract TookPlaceAtContract { get; protected set; }
    public virtual Location ParticipantCurrentLocation { get; protected set; }
    public virtual Contract? ParticipantCurrentContract { get; protected set; }
    public EnrolmentStatus ParticipantStatus { get; protected set; }
    public string? AdditionalInformation { get; protected set; }
    public DateTime Completed { get; protected set; }
    public string TenantId { get; protected set; }
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