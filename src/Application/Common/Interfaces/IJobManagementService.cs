namespace Cfo.Cats.Application.Common.Interfaces;

/// <summary>
/// Provides an abstraction for managing Quartz scheduled jobs, regardless of whether
/// they run in-process (CATS) or in a separate Worker process.
/// </summary>
public interface IJobManagementService
{
    /// <summary>Returns a summary of all registered Quartz jobs and their current state.</summary>
    Task<IReadOnlyList<JobSummary>> GetJobsAsync(CancellationToken cancellationToken = default);

    /// <summary>Triggers a job to run immediately, outside its normal schedule.</summary>
    Task TriggerJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>Pauses a job so its triggers are suspended.</summary>
    Task PauseJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>Resumes a previously paused job.</summary>
    Task ResumeJobAsync(string jobName, CancellationToken cancellationToken = default);

    /// <summary>Returns current metadata about the scheduler itself.</summary>
    Task<SchedulerInfo> GetSchedulerInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>Puts the scheduler into standby (pauses all firing).</summary>
    Task StandbyAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts (or resumes from standby) the scheduler.</summary>
    Task StartAsync(CancellationToken cancellationToken = default);
}

/// <summary>A point-in-time snapshot of a Quartz job's state.</summary>
public sealed record JobSummary(
    string Name,
    string? Group,
    string? Description,
    /// <summary>
    /// The trigger state as reported by Quartz: Normal, Paused, Complete, Error, Blocked, None,
    /// or "Executing" when the job is currently running.
    /// </summary>
    string Status,
    DateTimeOffset? NextFireTime,
    DateTimeOffset? PreviousFireTime
);

/// <summary>Point-in-time metadata about the Quartz scheduler.</summary>
public sealed record SchedulerInfo(
    string Name,
    bool IsStarted,
    bool IsInStandby,
    bool IsShutdown
);
