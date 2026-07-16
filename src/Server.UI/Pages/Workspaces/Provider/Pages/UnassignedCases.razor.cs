using Cfo.Cats.Server.UI.Components.Dashboard;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public partial class UnassignedCases
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    private bool _includeTransferIn = true;
    private string? _keyword;
    private int? _enrolmentStatus;
    private int? _locationId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cached = await SessionStorage.GetAsync<UnassignedCasesSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            RestoreState(
                sd.VisualMode,
                sd.IncludeTransferIn,
                sd.Keyword,
                sd.EnrolmentStatus,
                sd.LocationId,
                sd.TenantId,
                sd.UserId);
        }
    }

    private void RestoreState(
        bool visualMode,
        bool includeTransferIn,
        string? keyword,
        int? enrolmentStatus,
        int? locationId,
        string? tenantId,
        string? userId)
    {
        VisualMode = visualMode;
        _includeTransferIn = includeTransferIn;
        _keyword = keyword;
        _enrolmentStatus = enrolmentStatus;
        _locationId = locationId;

        if (CanFilter is false)
        {
            return;
        }

        OnTenantSelected(string.IsNullOrWhiteSpace(tenantId) ? null : tenantId);
        OnUserSelected(string.IsNullOrWhiteSpace(userId) ? null : userId);
    }

    private async Task OnVisualModeChanged(bool visualMode)
    {
        VisualMode = visualMode;
        await SaveSessionState();
    }

    private async Task OnFiltersChanged(UnassignedCasesDashboardFilters filters)
    {
        _includeTransferIn = filters.IncludeTransferIn;
        _keyword = filters.Keyword;
        _enrolmentStatus = filters.EnrolmentStatus;
        _locationId = filters.LocationId;
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
        => SessionStorage.SetAsync(UnassignedCasesSessionData.FromState(
            VisualMode,
            _includeTransferIn,
            _keyword,
            _enrolmentStatus,
            _locationId,
            SelectedTenantId,
            SelectedUserId));
}
