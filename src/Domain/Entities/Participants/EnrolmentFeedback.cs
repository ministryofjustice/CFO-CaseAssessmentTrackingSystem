using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.Entities.Participants;

public class EnrolmentFeedback : OwnerPropertyEntity<Guid>
{
    public string ParticipantId { get;  init; }

    public string RecipientUserId { get; init; }
    public ApplicationUser? RecipientUser { get; init; }

    public string Message { get; init; } = null!;

    public FeedbackOutcome Outcome { get; private set; }
    public FeedbackStage Stage { get; init; }

    public DateTime EnrolmentProcessedDate { get; init; }

    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string TenantId { get; protected init; }
    public ApplicationUser? CreatedByUser { get; private set; }
    
    public required string EnrolmentFeedbackReason { get; init; }
    public virtual Participant? Participant { get; protected init; }
    public virtual Tenant? Tenant { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EnrolmentFeedback()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    public static EnrolmentFeedback Create(
        string participantId,
        string recipientUserId,
        string message,
        FeedbackOutcome outcome,
        FeedbackStage stage,
        DateTime enrolmentProcessedDate,
        string createdBy,
        string tenantId, 
        string enrolmentFeedbackReason)
    {
       var feedback = new EnrolmentFeedback
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = participantId,
            RecipientUserId = recipientUserId,
            Message = message,
            Outcome = outcome,
            Stage = stage,
            EnrolmentProcessedDate = enrolmentProcessedDate,
            CreatedBy = createdBy,
            Created = DateTime.UtcNow,
            IsRead = false,
            TenantId = tenantId,
            EnrolmentFeedbackReason = enrolmentFeedbackReason
        };

         feedback.AddDomainEvent(
             new EnrolmentFeedbackCreatedDomainEvent(feedback));

        return feedback;
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        }
        
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}