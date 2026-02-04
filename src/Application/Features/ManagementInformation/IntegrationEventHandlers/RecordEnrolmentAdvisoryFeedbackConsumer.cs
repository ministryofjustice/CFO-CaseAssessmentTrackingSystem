using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordEnrolmentAdvisoryFeedbackConsumer(IUnitOfWork unitOfWork, ILogger<RecordEnrolmentAdvisoryFeedbackConsumer> logger) 
    : IHandleMessages<ParticipantTransitionedIntegrationEvent>
{
    public async Task Handle(ParticipantTransitionedIntegrationEvent context)
    {
        logger.LogInformation("RecordEnrolmentAdvisoryFeedbackConsumer received event for ParticipantId: {ParticipantId}, From: {From}, To: {To}", 
            context.ParticipantId, context.From, context.To);

        if (context.From != EnrolmentStatus.SubmittedToAuthorityStatus.Name || 
            context.To != EnrolmentStatus.ApprovedStatus.Name)
        {
            logger.LogDebug("Ignoring transition from {From} to {To}", context.From, context.To);
            return;
        }

        var dbContext = unitOfWork.DbContext;

        var qa2Record = await (
            from q in dbContext.EnrolmentQa2Queue
            from n in q.Notes
            where q.ParticipantId == context.ParticipantId 
                && q.IsCompleted == true 
                && q.IsAccepted == true
                && (n.FeedbackType! == FeedbackType.Advisory || n.FeedbackType! == FeedbackType.AcceptedByException)
            orderby q.LastModified descending
            select new
            {
                QueueEntryId = q.Id,
                NoteId = EF.Property<int>(n, "Id"),
                q.TenantId,
                q.ParticipantId,
                q.SupportWorkerId,
                ProviderQaUserId = q.LastModifiedBy ?? string.Empty,
                CfoUserId = q.OwnerId ?? string.Empty,
                PqaSubmittedDate = q.LastModified ?? DateTime.UtcNow,
                ActionDate = q.Created ?? DateTime.UtcNow,
                n.Message,
                FeedbackType = (int?)n.FeedbackType,
                Queue = "QA2",
                SourceTable = "Enrolment.Qa2QueueNote",
                q.LastModified
            }).FirstOrDefaultAsync();

        var escRecord = await (
            from eq in dbContext.EnrolmentEscalationQueue
            from en in eq.Notes
            where eq.ParticipantId == context.ParticipantId 
                && eq.IsCompleted == true 
                && eq.IsAccepted == true
                && (en.FeedbackType! == FeedbackType.Advisory || en.FeedbackType! == FeedbackType.AcceptedByException)
            orderby eq.LastModified descending
            select new
            {
                QueueEntryId = eq.Id,
                NoteId = EF.Property<int>(en, "Id"),
                eq.TenantId,
                eq.ParticipantId,
                eq.SupportWorkerId,
                ProviderQaUserId = eq.LastModifiedBy ?? string.Empty,
                CfoUserId = eq.CreatedBy ?? string.Empty,
                PqaSubmittedDate = eq.LastModified ?? DateTime.UtcNow,
                ActionDate = eq.Created ?? DateTime.UtcNow,
                en.Message,
                FeedbackType = (int?)en.FeedbackType,
                Queue = "Escalation",
                SourceTable = "Enrolment.EscalationNote",
                eq.LastModified
            }).FirstOrDefaultAsync();

        var latestRecord = new[] { qa2Record, escRecord }
            .Where(r => r != null)
            .OrderByDescending(r => r!.LastModified)
            .FirstOrDefault();

        if (latestRecord == null)
        {
            logger.LogInformation("No QA2 or Escalation records found for ParticipantId: {ParticipantId}", context.ParticipantId);
            return;
        }

        logger.LogInformation("Found latest record from {Queue} queue for ParticipantId: {ParticipantId}, FeedbackType: {FeedbackType}", 
            latestRecord.Queue, context.ParticipantId, latestRecord.FeedbackType);

        var contract = await (
            from c in dbContext.Contracts
            where latestRecord.TenantId.StartsWith(c.Tenant!.Id)
            orderby c.Tenant!.Id.Length descending
            select c.Id
        ).FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(contract))
        {
            logger.LogWarning("Contract not found for TenantId: {TenantId}", latestRecord.TenantId);
            return;
        }

        var exists = await dbContext.ProviderFeedbackEnrolments
            .AnyAsync(pfe => 
                pfe.QueueEntryId == latestRecord.QueueEntryId && 
                pfe.NoteId == latestRecord.NoteId &&
                pfe.SourceTable == latestRecord.SourceTable);

        if (exists)
        {
            logger.LogInformation("ProviderFeedbackEnrolment already exists for QueueEntryId: {QueueEntryId}, NoteId: {NoteId}", 
                latestRecord.QueueEntryId, latestRecord.NoteId);
            return;
        }

        var feedback = latestRecord.Queue == "QA2"
            ? ProviderFeedbackEnrolment.CreateAdvisoryQA2Enrolment(
                latestRecord.QueueEntryId,
                latestRecord.NoteId,
                latestRecord.TenantId,
                contract,
                latestRecord.ParticipantId,
                latestRecord.SupportWorkerId,
                latestRecord.ProviderQaUserId,
                latestRecord.CfoUserId,
                latestRecord.PqaSubmittedDate,
                latestRecord.ActionDate,
                latestRecord.Message ?? string.Empty,
                latestRecord.FeedbackType)
            : ProviderFeedbackEnrolment.CreateAdvisoryEscalationEnrolment(
                latestRecord.QueueEntryId,
                latestRecord.NoteId,
                latestRecord.TenantId,
                contract,
                latestRecord.ParticipantId,
                latestRecord.SupportWorkerId,
                latestRecord.ProviderQaUserId,
                latestRecord.CfoUserId,
                latestRecord.PqaSubmittedDate,
                latestRecord.ActionDate,
                latestRecord.Message ?? string.Empty,
                latestRecord.FeedbackType);

        dbContext.ProviderFeedbackEnrolments.Add(feedback);
        await unitOfWork.SaveChangesAsync();
        
        logger.LogInformation("Successfully created ProviderFeedbackEnrolment for ParticipantId: {ParticipantId}, Queue: {Queue}", 
            context.ParticipantId, latestRecord.Queue);
    }
}
