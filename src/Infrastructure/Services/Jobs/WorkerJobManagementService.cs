using System.Net.Http.Json;
using Cfo.Cats.Application.Common.Interfaces;

namespace Cfo.Cats.Infrastructure.Services.Jobs;

/// <summary>
/// HTTP client implementation of <see cref="IJobManagementService"/> that delegates to the
/// standalone CATS Worker process via its job management REST API.
/// Registered when <c>Features:UseWorkerForJobs</c> is <c>true</c>.
/// </summary>
public class WorkerJobManagementService(HttpClient httpClient) : IJobManagementService
{
    public async Task<IReadOnlyList<JobSummary>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<List<JobSummary>>("/api/jobs", cancellationToken);
        return result ?? [];
    }

    public async Task TriggerJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/api/jobs/{Uri.EscapeDataString(jobName)}/trigger", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task PauseJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/api/jobs/{Uri.EscapeDataString(jobName)}/pause", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResumeJobAsync(string jobName, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync($"/api/jobs/{Uri.EscapeDataString(jobName)}/resume", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<SchedulerInfo> GetSchedulerInfoAsync(CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetFromJsonAsync<SchedulerInfo>("/api/scheduler", cancellationToken);
        return result ?? new SchedulerInfo("Unknown", false, false, true);
    }

    public async Task StandbyAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync("/api/scheduler/standby", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync("/api/scheduler/start", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
