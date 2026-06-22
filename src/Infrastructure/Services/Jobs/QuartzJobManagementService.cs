using Cfo.Cats.Application.Common.Interfaces;
using Quartz;
using Quartz.Impl.Matchers;

namespace Cfo.Cats.Infrastructure.Services.Jobs;

/// <summary>
/// In-process implementation of <see cref="IJobManagementService"/> that communicates
/// directly with the Quartz <see cref="ISchedulerFactory"/> running inside the same process.
/// Registered when <c>Features:UseWorkerForJobs</c> is <c>false</c> (default).
/// </summary>
public class QuartzJobManagementService(ISchedulerFactory schedulerFactory) : IJobManagementService
{
    public async Task<IReadOnlyList<JobSummary>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup(), cancellationToken);
        var executingJobs = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);
        var executingKeys = executingJobs.Select(j => j.JobDetail.Key).ToHashSet();

        var summaries = new List<JobSummary>(jobKeys.Count);

        foreach (var jobKey in jobKeys.OrderBy(k => k.Name))
        {
            var detail = await scheduler.GetJobDetail(jobKey, cancellationToken);
            var triggers = await scheduler.GetTriggersOfJob(jobKey, cancellationToken);

            DateTimeOffset? nextFireTime = null;
            DateTimeOffset? previousFireTime = null;
            string status = "None";

            if (triggers.Count > 0)
            {
                // Use the earliest next-fire trigger as the representative trigger
                var primaryTrigger = triggers
                    .OrderBy(t => t.GetNextFireTimeUtc())
                    .First();

                nextFireTime = primaryTrigger.GetNextFireTimeUtc();
                previousFireTime = primaryTrigger.GetPreviousFireTimeUtc();

                var triggerState = await scheduler.GetTriggerState(primaryTrigger.Key, cancellationToken);
                status = executingKeys.Contains(jobKey) ? "Executing" : triggerState.ToString();
            }

            summaries.Add(new JobSummary(
                jobKey.Name,
                jobKey.Group,
                detail?.Description,
                status,
                nextFireTime,
                previousFireTime
            ));
        }

        return summaries;
    }

    public async Task TriggerJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(new JobKey(jobName), cancellationToken);
    }

    public async Task PauseJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.PauseJob(new JobKey(jobName), cancellationToken);
    }

    public async Task ResumeJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.ResumeJob(new JobKey(jobName), cancellationToken);
    }

    public async Task<SchedulerInfo> GetSchedulerInfoAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        return new SchedulerInfo(
            scheduler.SchedulerName,
            scheduler.IsStarted,
            scheduler.InStandbyMode,
            scheduler.IsShutdown
        );
    }

    public async Task StandbyAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.Standby(cancellationToken);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.Start(cancellationToken);
    }
}
