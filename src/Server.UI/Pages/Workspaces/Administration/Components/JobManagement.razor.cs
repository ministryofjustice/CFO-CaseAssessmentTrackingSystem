namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components;

public partial class JobManagement
{
    [Inject]
    private IJobManagementService JobManagementService { get; set; } = null!;

    private IReadOnlyList<JobSummary> _jobs = [];
    private SchedulerInfo? _schedulerInfo;
    private bool _isTriggering = false;

    protected override async Task OnInitializedAsync() => await RefreshAsync();

    private async Task RefreshAsync()
    {
        _schedulerInfo = await JobManagementService.GetSchedulerInfoAsync();
        _jobs = await JobManagementService.GetJobsAsync();
    }

    private async Task TriggerJob(string jobName)
    {
        try
        {
            _isTriggering = true;
            await JobManagementService.TriggerJobAsync(jobName);
            Snackbar.Add($"Job '{jobName}' triggered successfully", Severity.Info);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Job '{jobName}' failed. {ex.Message}", Severity.Error);
        }
        finally
        {
            _isTriggering = false;
            StateHasChanged();
        }
    }

    private async Task PauseScheduler()
    {
        try
        {
            await JobManagementService.StandbyAsync();
            _schedulerInfo = await JobManagementService.GetSchedulerInfoAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to pause scheduler: {ex.Message}", Severity.Error);
        }
    }

    private async Task StartScheduler()
    {
        try
        {
            await JobManagementService.StartAsync();
            _schedulerInfo = await JobManagementService.GetSchedulerInfoAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to resume scheduler: {ex.Message}", Severity.Error);
        }
    }

    private string GetSchedulerStatus()
    {
        if (_schedulerInfo is null)
        {
            return "Scheduler not available";
        }

        if (_schedulerInfo.IsShutdown)
        {
            return "Scheduler is shutdown";
        }

        if (_schedulerInfo.IsInStandby)
        {
            return $"Scheduler '{_schedulerInfo.Name}' is in standby mode";
        }

        if (_schedulerInfo.IsStarted)
        {
            return $"Scheduler '{_schedulerInfo.Name}' is active";
        }

        return "Unknown";
    }
}
