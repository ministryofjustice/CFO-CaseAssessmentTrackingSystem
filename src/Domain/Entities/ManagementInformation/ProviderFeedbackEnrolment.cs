namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ProviderFeedbackEnrolment
{
#pragma warning disable CS8618 
    private ProviderFeedbackEnrolment(){}
#pragma warning restore CS8618    

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required DateTime CreatedOn { get; set; }    

    public required string SourceTable { get; set; }

    public required string Queue { get; set; }

    public required Guid QueueEntryId { get; set; }

    public required int NoteId { get; set; }

    public required string TenantId { get; set; }

    public required string ContractId { get; set; }

    public required string ParticipantId { get; set; }

    public required string SupportWorkerId { get; set; }

    public required string ProviderQaUserId { get; set; }

    public required string CfoUserId { get; set; }

    public required DateTime PqaSubmittedDate { get; set; }

    public required DateTime ActionDate { get; set; }

    public string Message { get; set; }

    public int? FeedbackType { get; set; }

    public static ProviderFeedbackEnrolment CreateReturnedQA2Enrolment(
        Guid queueEntryId,
        int noteId,
        string tenantId,
        string contractId,
        string participantId,
        string supportWorkerId,
        string providerQaUserId,
        string cfoUserId,
        DateTime pqaSubmittedDate,
        DateTime actionDate,
        string message,
        int? feedbackType)
    {
        return new ProviderFeedbackEnrolment()
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            Queue = "QA2",
            SourceTable = "Enrolment.Qa2QueueNote",
            QueueEntryId = queueEntryId,
            NoteId = noteId,
            TenantId = tenantId,
            ContractId = contractId,
            ParticipantId = participantId,
            SupportWorkerId = supportWorkerId,
            ProviderQaUserId = providerQaUserId,
            CfoUserId = cfoUserId,
            PqaSubmittedDate = pqaSubmittedDate,
            ActionDate = actionDate,
            Message = message,
            FeedbackType = feedbackType
        };
    }
    public static ProviderFeedbackEnrolment CreateReturnedEscalationEnrolment(
        Guid queueEntryId,
        int noteId,
        string tenantId,
        string contractId,
        string participantId,
        string supportWorkerId,
        string providerQaUserId,
        string cfoUserId,
        DateTime pqaSubmittedDate,
        DateTime actionDate,
        string message,
        int? feedbackType)
    {
        return new ProviderFeedbackEnrolment()
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            Queue = "Escalation",
            SourceTable = "Enrolment.EscalationNote",
            QueueEntryId = queueEntryId,
            NoteId = noteId,
            TenantId = tenantId,
            ContractId = contractId,
            ParticipantId = participantId,
            SupportWorkerId = supportWorkerId,
            ProviderQaUserId = providerQaUserId,
            CfoUserId = cfoUserId,
            PqaSubmittedDate = pqaSubmittedDate,
            ActionDate = actionDate,
            Message = message,
            FeedbackType = feedbackType
        };
    }    
    public static ProviderFeedbackEnrolment CreateAdvisoryQA2Enrolment(
        Guid queueEntryId,
        int noteId,
        string tenantId,
        string contractId,
        string participantId,
        string supportWorkerId,
        string providerQaUserId,
        string cfoUserId,
        DateTime pqaSubmittedDate,
        DateTime actionDate,
        string message,
        int? feedbackType)
    {
        return new ProviderFeedbackEnrolment()
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            Queue = "QA2",
            SourceTable = "Enrolment.Qa2QueueNote",
            QueueEntryId = queueEntryId,
            NoteId = noteId,
            TenantId = tenantId,
            ContractId = contractId,
            ParticipantId = participantId,
            SupportWorkerId = supportWorkerId,
            ProviderQaUserId = providerQaUserId,
            CfoUserId = cfoUserId,
            PqaSubmittedDate = pqaSubmittedDate,
            ActionDate = actionDate,
            Message = message,
            FeedbackType = feedbackType
        };
    }
    public static ProviderFeedbackEnrolment CreateAdvisoryEscalationEnrolment(
        Guid queueEntryId,
        int noteId,
        string tenantId,
        string contractId,
        string participantId,
        string supportWorkerId,
        string providerQaUserId,
        string cfoUserId,
        DateTime pqaSubmittedDate,
        DateTime actionDate,
        string message,
        int? feedbackType)
    {
        return new ProviderFeedbackEnrolment()
        {
            Id = Guid.CreateVersion7(),
            CreatedOn = DateTime.UtcNow,
            Queue = "Escalation",
            SourceTable = "Enrolment.EscalationNote",
            QueueEntryId = queueEntryId,
            NoteId = noteId,
            TenantId = tenantId,
            ContractId = contractId,
            ParticipantId = participantId,
            SupportWorkerId = supportWorkerId,
            ProviderQaUserId = providerQaUserId,
            CfoUserId = cfoUserId,
            PqaSubmittedDate = pqaSubmittedDate,
            ActionDate = actionDate,
            Message = message,
            FeedbackType = feedbackType
        };
    }    
}