using Cfo.Cats.Application.Features.Participants.MessageBus;
using Quartz;
using Rebus.Bus;

namespace Cfo.Cats.Infrastructure.Jobs;

public class SyncParticipantsJob(
    ILogger<SyncParticipantsJob> logger,
    IUnitOfWork unitOfWork,
    IBus bus) : IJob
{

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


    public static readonly JobKey Key = new JobKey(name: nameof(SyncParticipantsJob));
    public static readonly string Description = "A job to synchronise participant information retrieved by the Candidate Service";

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
                ["JobName"] = Key.Name,
                ["JobGroup"] = Key.Group ?? "Default",
                ["JobInstance"] = Guid.NewGuid().ToString()
        }))

        if (context.RefireCount > 3)
        {
            logger.LogWarning("Quartz Job - {Key}: failed to complete within 3 tries, aborting...", Key.Name);
            return;
        }

        if (await _semaphore.WaitAsync(TimeSpan.Zero) == false)
        {
            // Job is already running, skip this execution
            logger.LogInformation("Quartz Job - {Key}: is already running. Skipping this call", Key.Name);
            return;
        }

        try
        {
            var participants = await unitOfWork.DbContext.Participants
                .AsNoTracking()
                .OrderBy(p => p.LastSyncDate)
                .Select(p => p.Id)
                .ToArrayAsync();

            foreach (var participant in participants.Select(p => new SyncParticipantCommand(p)))
            {
                // this is not ideal. We should be SENDING commands, not publishing them
                // but that is a wider architectural problem.
                await bus.Publish( participant );
            }
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"Quartz Job - {Key}: An unexpected error occurred executing job",
                refireImmediately: true, cause: ex);
        }
        finally
        {
            _semaphore.Release();
        }

        logger.LogInformation($"Job {Key} completed");

    }

}
