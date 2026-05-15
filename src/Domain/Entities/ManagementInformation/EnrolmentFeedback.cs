using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class EnrolmentFeedback : OwnerPropertyEntity<Guid>
{
   public string ParticipantId { get; private set; }
    public string Message { get; private set; } 

    public FeedbackOutcome Qa1Outcome { get; private set;}

    public DateTime Qa1Date { get;set; }

    public FeedbackOutcome Outcome { get; private set; }
    public FeedbackStage Stage { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public required string EnrolmentFeedbackReason { get; init; } 
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EnrolmentFeedback()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static EnrolmentFeedback Create(
        string participantId,
        string qa1UserId,
        string message,
        FeedbackOutcome qa1Outcome,
        FeedbackOutcome outcome,
        FeedbackStage stage,
        DateTime qa1Date,
        string enrolmentFeedbackReason)
    {
        var feedback = new EnrolmentFeedback
        {
            Id = Guid.CreateVersion7(),
            ParticipantId = participantId,
            OwnerId = qa1UserId,
            Message = message,
            Outcome = outcome,
            Stage = stage,
            Qa1Date = qa1Date,
            IsRead = false,
            Qa1Outcome = qa1Outcome,
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