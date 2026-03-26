using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordActivityAdvisoryFeedbackConsumer(IUnitOfWork unitOfWork, ILogger<RecordActivityAdvisoryFeedbackConsumer> logger) 
    : IHandleMessages<ActivityTransitionedIntegrationEvent>
{
    public async Task Handle(ActivityTransitionedIntegrationEvent context)
    {
        logger.LogInformation("RecordActivityAdvisoryFeedbackConsumer received event for ActivityId: {ActivityId}, From: {From}, To: {To}", 
            context.ActivityId, context.From, context.To);

        if (context.From != ActivityStatus.SubmittedToAuthorityStatus.Name || 
            context.To != ActivityStatus.ApprovedStatus.Name)
        {
            logger.LogDebug("Ignoring transition from {From} to {To}", context.From, context.To);
            return;
        }

        var dbContext = unitOfWork.DbContext;

        var activity = await dbContext.Activities
            .Where(a => a.Id == context.ActivityId)
            .Select(a => new 
            { 
                a.Id,
                a.Type,
                a.Definition,
                a.ParticipantId,
                a.TookPlaceAtContract,
                a.OwnerId
            })
            .FirstOrDefaultAsync();

        if (activity == null)
        {
            logger.LogWarning("Activity {ActivityId} not found", context.ActivityId);
            return;
        }
        
        if (!activity.Definition.RequiresQa)
        {
            logger.LogInformation("Skipping: ActivityDefinition {ActivityDefinition} does not require QA for ActivityId: {ActivityId}", 
                activity.Definition.Name, context.ActivityId);
            return;            
        }

        logger.LogInformation("Processing activity {ActivityId} of type {Type} for participant {ParticipantId}", 
            activity.Id, activity.Type.Value, activity.ParticipantId);

        var qa2Record = await (
            from q in dbContext.ActivityQa2Queue
            from n in q.Notes
            let pqaSubmission = dbContext.ActivityPqaQueue
                .Where(pqa => pqa.ActivityId == q.ActivityId
                    && pqa.LastModified < q.Created)
                .OrderByDescending(pqa => pqa.LastModified)
                .FirstOrDefault()
            where q.ActivityId == context.ActivityId 
                && q.IsCompleted == true 
                && q.IsAccepted == true
                && n.IsExternal == true
                && (n.FeedbackType! == FeedbackType.Advisory || n.FeedbackType! == FeedbackType.AcceptedByException)
                && q.Id == EF.Property<Guid>(n, "ActivityQa2QueueEntryId")
            orderby q.LastModified descending
            select new
            {
                QueueEntryId = q.Id,
                NoteId = EF.Property<int>(n, "Id"),
                q.TenantId,
                q.ParticipantId,
                q.SupportWorkerId,
                ProviderQaUserId = pqaSubmission != null ? pqaSubmission.LastModifiedBy ?? string.Empty : string.Empty,
                CfoUserId = n.CreatedBy ?? string.Empty,
                PqaSubmittedDate = pqaSubmission != null ? pqaSubmission.LastModified ?? DateTime.UtcNow : DateTime.UtcNow,
                ActionDate = n.Created ?? DateTime.UtcNow,
                n.Message,
                FeedbackType = (int?)n.FeedbackType,
                Queue = "QA2",
                SourceTable = "Activities.Qa2QueueNote",
                q.LastModified
            }).FirstOrDefaultAsync();

        var escRecord = await (
            from aq in dbContext.ActivityEscalationQueue
            from en in aq.Notes
            let pqaSubmission = dbContext.ActivityPqaQueue
                .Where(pqa => pqa.ActivityId == aq.ActivityId
                    && pqa.LastModified < aq.Created)
                .OrderByDescending(pqa => pqa.LastModified)
                .FirstOrDefault()
            where aq.ActivityId == context.ActivityId 
                && aq.IsCompleted == true 
                && aq.IsAccepted == true
                && en.IsExternal == true
                && (en.FeedbackType! == FeedbackType.Advisory || en.FeedbackType! == FeedbackType.AcceptedByException)
                && aq.Id == EF.Property<Guid>(en, "ActivityEscalationQueueEntryId")
            orderby aq.LastModified descending
            select new
            {
                QueueEntryId = aq.Id,
                NoteId = EF.Property<int>(en, "Id"),
                aq.TenantId,
                aq.ParticipantId,
                aq.SupportWorkerId,
                ProviderQaUserId = pqaSubmission != null ? pqaSubmission.LastModifiedBy ?? string.Empty : string.Empty,
                CfoUserId = en.CreatedBy ?? string.Empty,
                PqaSubmittedDate = pqaSubmission != null ? pqaSubmission.LastModified ?? DateTime.UtcNow : DateTime.UtcNow,
                ActionDate = en.Created ?? DateTime.UtcNow,
                en.Message,
                FeedbackType = (int?)en.FeedbackType,
                Queue = "Escalation",
                SourceTable = "Activities.EscalationNote",
                aq.LastModified
            }).FirstOrDefaultAsync();

        var latestRecord = new[] { qa2Record, escRecord }
            .Where(r => r != null)
            .OrderByDescending(r => r!.LastModified)
            .FirstOrDefault();

        if (latestRecord == null)
        {
            logger.LogInformation("No QA2 or Escalation records found for ActivityId: {ActivityId}", context.ActivityId);
            return;
        }

        logger.LogInformation("Found latest record from {Queue} queue for ActivityId: {ActivityId}, FeedbackType: {FeedbackType}", 
            latestRecord.Queue, context.ActivityId, latestRecord.FeedbackType);

        var contract = await (
            from c in dbContext.Contracts
            where c.Id == activity.TookPlaceAtContract.Id
            orderby c.Tenant!.Id.Length descending
            select c.Id
        ).FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(contract))
        {
            logger.LogWarning("Contract not found for TenantId: {TenantId}", latestRecord.TenantId);
            return;
        }

        var exists = await dbContext.ProviderFeedbackActivities
            .AnyAsync(pfa => 
                pfa.QueueEntryId == latestRecord.QueueEntryId && 
                pfa.NoteId == latestRecord.NoteId &&
                pfa.SourceTable == latestRecord.SourceTable);

        if (exists)
        {
            logger.LogInformation("ProviderFeedbackActivity already exists for QueueEntryId: {QueueEntryId}, NoteId: {NoteId}", 
                latestRecord.QueueEntryId, latestRecord.NoteId);
            return;
        }

        var feedback = latestRecord.Queue == "QA2"
            ? ProviderFeedbackActivity.CreateAdvisoryQA2Activity(
                latestRecord.QueueEntryId,
                latestRecord.NoteId,
                activity.Id.ToString(),
                activity.Type.Value,
                latestRecord.TenantId,
                contract,
                latestRecord.ParticipantId ?? string.Empty,
                latestRecord.SupportWorkerId,
                latestRecord.ProviderQaUserId,
                latestRecord.CfoUserId,
                latestRecord.PqaSubmittedDate,
                latestRecord.ActionDate,
                latestRecord.Message ?? string.Empty,
                latestRecord.FeedbackType)
            : ProviderFeedbackActivity.CreateAdvisoryEscalationActivity(
                latestRecord.QueueEntryId,
                latestRecord.NoteId,
                activity.Id.ToString(),
                activity.Type.Value,
                latestRecord.TenantId,
                contract,
                latestRecord.ParticipantId ?? string.Empty,
                latestRecord.SupportWorkerId,
                latestRecord.ProviderQaUserId,
                latestRecord.CfoUserId,
                latestRecord.PqaSubmittedDate,
                latestRecord.ActionDate,
                latestRecord.Message ?? string.Empty,
                latestRecord.FeedbackType);

        dbContext.ProviderFeedbackActivities.Add(feedback);
        await unitOfWork.SaveChangesAsync();
        
        logger.LogInformation("Successfully created ProviderFeedbackActivity for ActivityId: {ActivityId}, Queue: {Queue}", 
            context.ActivityId, latestRecord.Queue);

    }
}
