using Cfo.Cats.Application.Features.Dashboard.Queries;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Components;

public partial class ArchiveRecurrencesDashboard
{
    [Inject] private IContractService ContractService { get; set; } = null!;
    [Inject] private CatsSessionStorage SessionStorage { get; set; } = null!;

    private MudDateRangePicker _picker = null!;
    private DateRange _dateRange = new(
        DateTime.Today.AddDays(-29),
        DateTime.Today);

    private int _threshold = 5;
    private string? _contractId;
    private string? _expandedParticipantId;
    private bool _applyingFilters;

    protected override async Task OnInitializedAsync()
    {
        var cached = await SessionStorage.GetAsync<ArchiveRecurrencesSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            _dateRange = new DateRange(sd.StartDate, sd.EndDate);
            _threshold = sd.Threshold is >= 1 and <= 20
                ? sd.Threshold
                : 5;
            _contractId = sd.ContractId;
        }

        await base.OnInitializedAsync();
    }

    protected override IQuery<Result<ArchiveRecurrencesDto>> CreateQuery()
        => new GetArchiveRecurrences.Query
        {
            StartDate = _dateRange.Start ?? DateTime.Today.AddDays(-29),
            EndDate = _dateRange.End ?? DateTime.Today,
            ContractId = _contractId,
            Threshold = _threshold
        };

    private async Task ApplyFilters(Action updateFilters)
    {
        if (Loading || _applyingFilters || IsDisposed)
        {
            return;
        }

        _applyingFilters = true;
        try
        {
            updateFilters();
            _expandedParticipantId = null;
            await SaveSessionState();
            await RefreshAsync();
        }
        finally
        {
            _applyingFilters = false;
        }
    }

    private Task SaveSessionState()
        => SessionStorage.SetAsync(
            ArchiveRecurrencesSessionData.FromState(
                _dateRange,
                _threshold,
                _contractId));

    private string GetSelectedContractLabel()
    {
        if (string.IsNullOrWhiteSpace(_contractId))
        {
            return "Current contract";
        }

        return ContractService
                   .GetVisibleContracts(CurrentUser.TenantId!)
                   .FirstOrDefault(x => x.Id == _contractId)?
                   .Name
               ?? _contractId;
    }

    private Task OnDateRangeChanged(DateRange? dateRange)
        => dateRange is null ? Task.CompletedTask : ApplyFilters(() => _dateRange = dateRange);

    private Task OnContractChanged(string? contractId)
        => ApplyFilters(() => _contractId = contractId);

    private Task IncreaseThreshold()
    {
        if (_threshold >= 20)
        {
            return Task.CompletedTask;
        }

        return ApplyFilters(() => _threshold++);
    }

    private Task DecreaseThreshold()
    {
        if (_threshold <= 1)
        {
            return Task.CompletedTask;
        }

        return ApplyFilters(() => _threshold--);
    }

    private bool IsParticipantExpanded(string participantId)
        => _expandedParticipantId == participantId;

    private void ToggleParticipantExpanded(string participantId)
        => _expandedParticipantId = _expandedParticipantId == participantId
            ? null
            : participantId;

    private static string GetEpisodeDuration(
        ArchiveEpisodeDto episode)
    {
        var end = episode.To ?? DateTime.UtcNow;
        var days = Math.Max(0, (int)Math.Ceiling((end - episode.From).TotalDays));
        return $"{days} {(days == 1 ? "day" : "days")}";
    }
}
