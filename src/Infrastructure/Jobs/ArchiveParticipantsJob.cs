using Cfo.Cats.Domain.Common.Enums;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;
public class ArchiveParticipantsJob(
    ILogger<ArchiveParticipantsJob> logger,
    IUnitOfWork unitOfWork) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(ArchiveParticipantsJob));
    public static readonly string Description = "A job to archive participants that have been inactive in the data feed for the last 30 days.";

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["JobName"] = Key.Name,
            ["JobGroup"] = Key.Group ?? "Default",
            ["JobInstance"] = Guid.NewGuid().ToString()
        }))
        {
            if (context.RefireCount > 3)
            {
                logger.LogWarning($"Failed to complete {Key.Name} within 3 tries, aborting...");
                return;
            }
        }
        try
        {
            logger.LogInformation("Starting archive of inactive participants");

            var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));

            var participantsToDeactivate = await unitOfWork.DbContext.Participants
                .IgnoreAutoIncludes()
                .Where(p => p.DeactivatedInFeed < thirtyDaysAgo)
                .Where(p => p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
                .ToListAsync(context.CancellationToken);

            if(participantsToDeactivate is not { Count: > 0 })
            {
                logger.LogInformation("No participants to archive");
                return;
            }

            foreach (var participant in participantsToDeactivate)
            {
                participant.TransitionTo(EnrolmentStatus.ArchivedStatus, "Post license case closure period (30 days) elapsed", null);
                logger.LogInformation("Archived participant {Id}", participant.Id);
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);

            logger.LogInformation("Archived {deactivated} participant(s)", participantsToDeactivate.Count);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Quartz job {Key} failed", Key.Name);
            throw new JobExecutionException(msg: $"An unexpected error occurred executing {Key.Name} job", refireImmediately: true, cause: ex);
        }

    }
}
