using Cfo.Cats.Application;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-GB");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-GB");

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddWorkerInfrastructure(builder.Configuration, builder.Environment);

builder.AddServiceDefaults();

var app = builder.Build();

app.UseRequestTimeouts();
app.UseOutputCache();

app.MapDefaultEndpoints();

// ─── Job Management API ────────────────────────────────────────────────────────
// These endpoints are called by CATS when Features:UseWorkerForJobs = true,
// allowing the UI to inspect and control Quartz jobs running in this Worker process.

app.MapGet("/api/jobs", async (IJobManagementService jobs, CancellationToken ct) =>
    Results.Ok(await jobs.GetJobsAsync(ct)))
    .WithName("GetJobs")
    .WithSummary("Returns the current state of all registered Quartz jobs.");

app.MapPost("/api/jobs/{name}/trigger", async (string name, IJobManagementService jobs, CancellationToken ct) =>
{
    await jobs.TriggerJobAsync(name, ct);
    return Results.Ok();
})
.WithName("TriggerJob")
.WithSummary("Triggers a job to execute immediately, outside its normal cron schedule.");

app.MapPost("/api/jobs/{name}/pause", async (string name, IJobManagementService jobs, CancellationToken ct) =>
{
    await jobs.PauseJobAsync(name, ct);
    return Results.Ok();
})
.WithName("PauseJob")
.WithSummary("Pauses a job's triggers so it will not fire until resumed.");

app.MapPost("/api/jobs/{name}/resume", async (string name, IJobManagementService jobs, CancellationToken ct) =>
{
    await jobs.ResumeJobAsync(name, ct);
    return Results.Ok();
})
.WithName("ResumeJob")
.WithSummary("Resumes a previously paused job.");

// ─── Scheduler API ─────────────────────────────────────────────────────────────

app.MapGet("/api/scheduler", async (IJobManagementService jobs, CancellationToken ct) =>
    Results.Ok(await jobs.GetSchedulerInfoAsync(ct)))
    .WithName("GetSchedulerInfo")
    .WithSummary("Returns the current state of the Quartz scheduler.");

app.MapPost("/api/scheduler/standby", async (IJobManagementService jobs, CancellationToken ct) =>
{
    await jobs.StandbyAsync(ct);
    return Results.Ok();
})
.WithName("StandbyScheduler")
.WithSummary("Puts the scheduler into standby mode (suspends all firing).");

app.MapPost("/api/scheduler/start", async (IJobManagementService jobs, CancellationToken ct) =>
{
    await jobs.StartAsync(ct);
    return Results.Ok();
})
.WithName("StartScheduler")
.WithSummary("Starts or resumes the scheduler from standby.");

await app.RunAsync();
