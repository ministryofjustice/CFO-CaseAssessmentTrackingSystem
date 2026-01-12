using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Activities;

public class ActivityFeedback : OwnerPropertyEntity<Guid>
{
    public Guid ActivityId { get; set; }
    public string? ParticipantId { get; protected set; }

    public string? RecipientUserId { get; set; } = null!;
    public ApplicationUser? RecipientUser { get; set; } = null!;

    public string Message { get; set; } = null!;
    public FeedbackOutcome Outcome { get; set; }
    public FeedbackStage Stage { get; set; }

    public DateTime? ActivityProcessedDate { get; private set; }

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public virtual Activity? Activity { get; private set; }
    public virtual Participant? Participant { get; protected set; }

    public static ActivityFeedback Create(
        Guid activityId,
        string participantId,
        string recipientUserId,
        string message,
        FeedbackOutcome outcome,
        FeedbackStage stage,
        DateTime? activityProcessedDate,
        string createdBy)
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
            IsRead = false
        };

        //To do
        // feedback.AddDomainEvent(
        //     new ActivityFeedbackCreatedDomainEvent(feedback));

        return feedback;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}