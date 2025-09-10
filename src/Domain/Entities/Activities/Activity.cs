using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Common.Exceptions;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Activities;

public abstract class Activity : OwnerPropertyEntity<Guid>
{
    public record ActivityContext(
        ActivityDefinition Definition,
        string ParticipantId,
        ObjectiveTask Task,
        Location TookPlaceAtLocation,
        Contract TookPlaceAtContract,
        Location ParticipantCurrentLocation,
        Contract? ParticipantCurrentContract,
        EnrolmentStatus ParticipantStatus,
        DateTime CommencedOn,
        string TenantId,
        string OwnerId,
        string? AdditionalInformation = null);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Activity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected Activity(ActivityContext context)
    {
        Definition = context.Definition;
        Category = context.Definition.Category;
        Type = context.Definition.Type;
        ParticipantId = context.ParticipantId;

        TookPlaceAtLocation = context.TookPlaceAtLocation;
        TookPlaceAtContract = context.TookPlaceAtContract;
        ParticipantCurrentLocation = context.ParticipantCurrentLocation;
        ParticipantCurrentContract = context.ParticipantCurrentContract;
        ParticipantStatus = context.ParticipantStatus;
        CommencedOn = context.CommencedOn;
        TenantId = context.TenantId;
        TaskId = context.Task.Id;
        ObjectiveId = context.Task.ObjectiveId;
        AdditionalInformation = context.AdditionalInformation;
        Status = ActivityStatus.PendingStatus;
        OwnerId = context.OwnerId;

        AddDomainEvent(new ActivityCreatedDomainEvent(this));
    }

    public ActivityDefinition Definition { get; protected set; }
    public ActivityCategory Category { get; init; }
    public virtual Participant? Participant { get; protected set; }
    public ActivityType Type { get; init; }
    public string ParticipantId { get; protected set; }
    public Guid TaskId { get; init; }
    public Guid ObjectiveId { get; init; }
    public virtual Location TookPlaceAtLocation { get; protected set; }
    public virtual Contract TookPlaceAtContract { get; protected set; }
    public virtual Location ParticipantCurrentLocation { get; protected set; }
    public virtual Contract? ParticipantCurrentContract { get; protected set; }
    public EnrolmentStatus ParticipantStatus { get; protected set; }
    public string? AdditionalInformation { get; protected set; }
    public DateTime CommencedOn { get; protected set; }
    public string TenantId { get; protected set; }
    public ActivityStatus Status { get; protected set; }
    public DateTime? ApprovedOn { get; protected set; }
    public string? CompletedBy { get; private set; }

    /// <summary>
    /// The justification for Abandoning Activity
    /// </summary>
    public string? AbandonJustification { get; private set; }

    /// <summary>
    /// The reason for Abandoning Activity
    /// </summary>
    public ActivityAbandonReason? AbandonReason { get; private set; }

    /// <summary>
    /// Who completed the Activity
    /// </summary>
    public string? CompletedBy { get; private set; }

    public virtual DateTime Expiry => CommencedOn.AddMonths(3);

    public Activity TransitionTo(ActivityStatus to)
    {
        if(Status.CanTransitionTo(to))
        {
            AddDomainEvent(new ActivityTransitionedDomainEvent(this, Status, to));
            Status = to;
            return this;
        }

        throw new InvalidActivityTransition(Status, to);
    }

    public Activity Approve(string? completedBy)
    {
        if (ApprovedOn.HasValue)
        {
            return this;
        }

        ApprovedOn = DateTime.UtcNow;
        CompletedBy = completedBy;

        TransitionTo(ActivityStatus.ApprovedStatus);
        
        return this;
    }

    public Activity Abandon(ActivityAbandonReason abandonReason, string? abandonJustification, string abandonedBy)
    {
        Status = ActivityStatus.AbandonedStatus;
        //CompletedOn = DateTime.UtcNow;
        AbandonReason = abandonReason;
        AbandonJustification = abandonJustification;
        CompletedBy = abandonedBy;
        ApprovedOn = DateTime.UtcNow;
        AddDomainEvent(new ActivityAbandonedDomainEvent(this));        
        return this;
    }

    public bool RequiresQa => Definition.RequiresQa;
}