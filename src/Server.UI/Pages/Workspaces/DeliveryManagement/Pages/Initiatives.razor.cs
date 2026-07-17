using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Initiatives
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    private bool _showActiveOnly;
    private InitiativeDto? _initiativeFilter;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        var cached = await SessionStorage.GetAsync<InitiativesSessionData>();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            RestoreState(sd.VisualMode, sd.ShowActiveOnly, sd.InitiativeFilter, sd.TenantId, sd.UserId);
        }
    }

    private void RestoreState(bool visualMode, bool showActiveOnly, InitiativeDto? initiativeFilter, string? tenantId, string? userId)
    {
        VisualMode = visualMode;
        _showActiveOnly = showActiveOnly;
        _initiativeFilter = initiativeFilter;

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

    private async Task OnShowActiveOnlyChanged(bool showActiveOnly)
    {
        _showActiveOnly = showActiveOnly;
        await SaveSessionState();
    }

    private async Task OnInitiativeFilterChanged(InitiativeDto? initiativeFilter)
    {
        _initiativeFilter = initiativeFilter;
        await SaveSessionState();
    }

    private async Task OnTenantSelectedWithSave(string? tenantId)
    {
        var previousTenantId = SelectedTenantId;
        OnTenantSelected(tenantId);

        if (previousTenantId != SelectedTenantId)
        {
            // Initiative options are tenant-scoped; reset selection when tenant scope changes.
            _initiativeFilter = null;
        }

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
        _initiativeFilter = null;
        await SaveSessionState();
    }

    private Task SaveSessionState()
        => SessionStorage.SetAsync(InitiativesSessionData.FromState(
            VisualMode,
            _showActiveOnly,
            _initiativeFilter,
            SelectedTenantId,
            SelectedUserId));
}
