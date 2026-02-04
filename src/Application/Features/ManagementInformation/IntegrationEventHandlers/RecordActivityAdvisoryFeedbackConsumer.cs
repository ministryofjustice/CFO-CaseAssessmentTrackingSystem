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
                a.ParticipantId,
                a.TenantId,
                a.OwnerId
            })
            .FirstOrDefaultAsync();

        if (activity == null)
        {
            logger.LogWarning("Activity {ActivityId} not found", context.ActivityId);
            return;
        }
        
        if(activity.Type != ActivityType.InterventionsAndServicesWraparoundSupport
                && activity.Type != ActivityType.Employment 
                && activity.Type != ActivityType.EducationAndTraining)
        {
            logger.LogInformation("Skipping: ActivityType {ActivityType} is not Employment, EducationAndTraining or InterventionsAndServicesWraparoundSupport for ActivityId: {ActivityId}", 
                activity.Type.Name, context.ActivityId);
            return;            
        }

        logger.LogInformation("Processing activity {ActivityId} of type {Type} for participant {ParticipantId}", 
            activity.Id, activity.Type.Value, activity.ParticipantId);

        var qa2Record = await (
            from q in dbContext.ActivityQa2Queue
            from n in q.Notes
            where q.ActivityId == context.ActivityId 
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
                SourceTable = "Activities.Qa2QueueNote",
                q.LastModified
            }).FirstOrDefaultAsync();

        var escRecord = await (
            from eq in dbContext.ActivityEscalationQueue
            from en in eq.Notes
            where eq.ActivityId == context.ActivityId 
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
                SourceTable = "Activities.EscalationNote",
                eq.LastModified
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
            where latestRecord.TenantId.StartsWith(c.Tenant!.Id)
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
