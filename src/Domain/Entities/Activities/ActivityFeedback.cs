using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityFeedback : OwnerPropertyEntity<Guid>
{
    public Guid ActivityId { get; set; }
    public string ParticipantId { get; protected set; }

    public string RecipientUserId { get; set; }
    public ApplicationUser? RecipientUser { get; set; }

    public string Message { get; set; } = null!;

    public FeedbackOutcome Outcome { get; set; }
    public FeedbackStage Stage { get; set; }

    public DateTime ActivityProcessedDate { get; private set; }

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string TenantId { get; protected set; }
    public ApplicationUser? CreatedByUser { get; private set; }

    public virtual Activity? Activity { get; private set; }
    public virtual Participant? Participant { get; protected set; }
    public virtual Tenant? Tenant { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ActivityFeedback()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    
    public static ActivityFeedback Create(
        Guid activityId,
        string participantId,
        string recipientUserId,
        string message,
        FeedbackOutcome outcome,
        FeedbackStage stage,
        DateTime activityProcessedDate,
        string createdBy,
        string tenantId )
    {
       var feedback = new ActivityFeedback
        {
            Id = Guid.CreateVersion7(),
            ActivityId = activityId,
            ParticipantId = participantId,
            RecipientUserId = recipientUserId,
            Message = message,
            Outcome = outcome,
            Stage = stage,
            ActivityProcessedDate = activityProcessedDate,
            CreatedBy = createdBy,
            Created = DateTime.UtcNow,
            IsRead = false,
            TenantId = tenantId
        };

         feedback.AddDomainEvent(
             new ActivityFeedbackCreatedDomainEvent(feedback));

        return feedback;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}