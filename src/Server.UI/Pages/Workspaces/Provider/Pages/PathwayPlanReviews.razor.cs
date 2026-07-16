using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public partial class PathwayPlanReviews
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    private bool _showOverdueOnly;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cached = await SessionStorage.GetAsync<PathwayPlanReviewsSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            RestoreState(sd.VisualMode, sd.ShowOverdueOnly, sd.TenantId, sd.UserId);
        }
    }

    private void RestoreState(bool visualMode, bool showOverdueOnly, string? tenantId, string? userId)
    {
        VisualMode = visualMode;
        _showOverdueOnly = showOverdueOnly;

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

    private async Task OnShowOverdueOnlyChanged(bool showOverdueOnly)
    {
        _showOverdueOnly = showOverdueOnly;
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
        => SessionStorage.SetAsync(PathwayPlanReviewsSessionData.FromState(
            VisualMode,
            _showOverdueOnly,
            SelectedTenantId,
            SelectedUserId));
}
