using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public partial class RecentApprovedActivities
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    private MudDateRangePicker _picker = null!;

    private DateRange _dateRange = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);

    private string PerformanceKey => $"{SelectionKey}|{_dateRange.Start?.Ticks ?? 0}|{_dateRange.End?.Ticks ?? 0}";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cached = await SessionStorage.GetAsync<RecentApprovedActivitiesSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            _dateRange = new DateRange(sd.StartDate, sd.EndDate);
            RestoreState(sd.VisualMode, sd.TenantId, sd.UserId);
        }
    }

    private void RestoreState(bool visualMode, string? tenantId, string? userId)
    {
        VisualMode = visualMode;

        if (CanFilter is false)
        {
            return;
        }

        OnTenantSelected(string.IsNullOrWhiteSpace(tenantId) ? null : tenantId);
        OnUserSelected(string.IsNullOrWhiteSpace(userId) ? null : userId);
    }

    private async Task OnDateRangeChanged(DateRange? dateRange)
    {
        _dateRange = dateRange ?? _dateRange;
        await SaveSessionState();
    }

    private async Task OnVisualModeChanged(bool visualMode)
    {
        VisualMode = visualMode;
        await SaveSessionState();
    }

    private async Task OnTenantSelectedWithSave(string? tenantId)
    {
        OnTenantSelected(tenantId);
        await SaveSessionState();
    }

    private async Task OnUserSelectedWithSave(string? userId)
    {
        OnUserSelected(userId);
        await SaveSessionState();
    }

    private async Task OnClearFilterWithSave()
    {
        OnClearFilter();
        await SaveSessionState();
    }

    private Task SaveSessionState()
        => SessionStorage.SetAsync(RecentApprovedActivitiesSessionData.FromState(
            _dateRange,
            VisualMode,
            SelectedTenantId,
            SelectedUserId));
}