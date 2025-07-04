using Quartz;
using Quartz.Impl.Matchers;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class JobManagement
{

    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    private List<JobDetailInfo> JobDetails = new();
    private bool _isTriggering = false;
    private string? _message = string.Empty;
    private IScheduler? _scheduler;

    protected override async Task OnInitializedAsync()
    {
        var factory = ServiceProvider.GetRequiredService<ISchedulerFactory>();
        _scheduler = await factory.GetScheduler();
        JobDetails = await GetAllJobs();
    }

    private async Task TriggerJob(string key)
    {
        try
        {
            _isTriggering = true;
            if (_scheduler == null)
            {
                throw new InvalidOperationException("Scheduler not initialized");
            }

            await _scheduler.TriggerJob(new JobKey(key));

            Snackbar.Add($"Job '{key}' triggered successfully", Severity.Info);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Job '{key}' failed. {ex.Message}", Severity.Error);
        }
        finally
        {
            _isTriggering = false;
            StateHasChanged();
        }
    }

    private async Task<List<JobDetailInfo>> GetAllJobs()
    {
        List<JobDetailInfo> jobDetails = [];

        if (_scheduler is not null)
        {
            var jobGroups = await _scheduler.GetJobGroupNames();
            foreach (var group in jobGroups)
            {
                var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(group));

                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = await _scheduler.GetJobDetail(jobKey);

                    var triggers = await _scheduler.GetTriggersOfJob(jobKey);

                    foreach (var trigger in triggers)
                    {
                        var triggerState = await _scheduler.GetTriggerState(trigger.Key);

                        jobDetails.Add(new JobDetailInfo
                        {
                            JobName = jobKey.Name,
                            Group = group,
                            Description = jobDetail!.Description ?? jobKey.Name,
                            TriggerState = triggerState.ToString(),
                            NextFireTime = trigger.GetNextFireTimeUtc()?.DateTime,
                            PreviousFireTime = trigger.GetPreviousFireTimeUtc()?.DateTime,
                        });
                    }
                }
            }
        }

        return jobDetails;
    }

    private async Task PauseScheduler()
    {
        if (_scheduler is null)
        {
            throw new InvalidOperationException("Scheduler not initialized");
        }

        await _scheduler.Standby();
    }

    private async Task StartScheduler()
    {
        if (_scheduler is null)
        {
            throw new InvalidOperationException("Scheduler not initialized");
        }

        await _scheduler.Start();
    }

    private string GetSchedulerStatus()
    {
        if (_scheduler is null)
        {
            return "Scheduler not defined";
        }

        if (_scheduler.InStandbyMode)
        {
            return "Scheduler is in standby mode";
        }

        if (_scheduler.IsShutdown)
        {
            return "Scheduler is shutdown";
        }

        if (_scheduler.IsStarted)
        {
            return "Scheduler is active";
        }

        return "Unknown";
    }

    public class JobDetailInfo
    {
        public string JobName { get; set; } = default!;
        public string Group { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string TriggerState { get; set; } = default!;
        public DateTime? NextFireTime { get; set; } = default!;
        public DateTime? PreviousFireTime { get; set; } = default!;
    }
}