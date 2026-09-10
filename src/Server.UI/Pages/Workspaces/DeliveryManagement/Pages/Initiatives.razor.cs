using Cfo.Cats.Application.Features.Dashboard.Queries;
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
            await RestoreState(sd.VisualMode, sd.ShowActiveOnly, sd.InitiativeFilter, sd.TenantId, sd.UserId);
        }
    }

    private async Task RestoreState(bool visualMode, bool showActiveOnly, InitiativeDto? initiativeFilter, string? tenantId, string? userId)
    {
        VisualMode = visualMode;
        _showActiveOnly = showActiveOnly;
        _initiativeFilter = initiativeFilter;

        if (CanFilter is false)
        {
            return;
        }

        await OnTenantSelected(string.IsNullOrWhiteSpace(tenantId) ? null : tenantId);
        OnUserSelected(string.IsNullOrWhiteSpace(userId) ? null : userId);
    }

    protected override async Task<IDictionary<string, string>> LoadUsersAsync()
    {
        var result = await GetNewMediator().Send(new GetInitiativeObjectiveAssignees.Query(CurrentUser)
        {
            TenantId = SelectedTenantId
        });

        return result is { Succeeded: true, Data: not null }
            ? result.Data.ToDictionary(a => a.Id, a => a.DisplayName)
            : new Dictionary<string, string>();
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
        await OnTenantSelected(tenantId);

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
        await OnClearFilter();
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
