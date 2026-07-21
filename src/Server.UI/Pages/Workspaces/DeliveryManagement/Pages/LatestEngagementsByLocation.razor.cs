using ApexCharts;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class LatestEngagementsByLocation
{
    private bool _loading = true;
    private bool _downloading;
    private bool _visualMode = true;
    private bool _canFilterTenant;
    private bool _isTenantLevel;
    private string? _selectedTenantName;

    private IDictionary<int, string> _locations = new Dictionary<int, string>();
    private string[] _engagementTypes = [];

    // Sentinel-backed filter selections: 0 / empty string represent "All".
    private int _selectedLocationId;
    private string _selectedEngagementType = string.Empty;

    private LatestEngagementsByLocationDto? _data;
    private MudTable<ParticipantEngagementDto> _table = null!;

    [Inject] private ILocationService LocationService { get; set; } = null!;
    [Inject] private IParticipantDialogService ParticipantDialogService { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    [CascadingParameter] public UserProfile CurrentUser { get; set; } = null!;
    [CascadingParameter(Name = "IsDarkMode")] private bool IsDarkMode { get; set; }

    private GetLatestEngagementsByLocation.Query Query { get; set; } = null!;

    private ApexChartOptions<LocationEngagementSummaryDto> _options = null!;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canFilterTenant = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.UserHasAdditionalRoles)).Succeeded;

        // Support workers see only their own cases; senior/additional-roles users see the whole tenant.
        _isTenantLevel = _canFilterTenant;

        _locations = LocationService.GetVisibleLocations(CurrentUser.TenantId!)
            .ToDictionary(k => k.Id, e => e.Name);

        Query = new GetLatestEngagementsByLocation.Query
        {
            CurrentUser = CurrentUser,
            JustMyCases = _isTenantLevel is false
        };

        _options = BuildChartOptions();

        var typesResult = await GetNewMediator().Send(new GetEngagementTypes.Query { CurrentUser = CurrentUser });
        if (typesResult is { Succeeded: true, Data: not null })
        {
            _engagementTypes = typesResult.Data;
        }

        await OnRefresh();
    }

    private ApexChartOptions<LocationEngagementSummaryDto> BuildChartOptions() => new()
    {
        Chart = new Chart { Stacked = true },
        Legend = new Legend { Show = true, ShowForSingleSeries = true },
        Yaxis = [new YAxis { Min = 0, ForceNiceScale = true }],
        Theme = new Theme { Mode = IsDarkMode ? Mode.Dark : Mode.Light }
    };

    private async Task OnRefresh()
    {
        // In table mode the MudTable's ServerData runs the (single) combined query; just reload it.
        // In visual mode there is no table, so run the query directly to populate the chart.
        if (_visualMode is false && _table is not null)
        {
            await _table.ReloadServerData();
            return;
        }

        try
        {
            _loading = true;
            StateHasChanged();

            Query.PageNumber = 1;
            var result = await GetNewMediator().Send(Query, ComponentCancellationToken);

            if (result is { Succeeded: true, Data: not null })
            {
                _data = result.Data;
            }
            else
            {
                _data = null;
                if (result?.ErrorMessage is not null)
                {
                    Snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Component disposed or navigated away; nothing to do.
        }
        finally
        {
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task<TableData<ParticipantEngagementDto>> ServerReload(TableState state, CancellationToken cancellationToken)
    {
        Query.PageNumber = state.Page + 1;
        Query.PageSize = state.PageSize;
        Query.OrderBy = string.IsNullOrWhiteSpace(state.SortLabel) ? "EngagedOn" : state.SortLabel;
        Query.SortDirection = state.SortDirection == SortDirection.Descending
            ? SortDirection.Descending.ToString()
            : SortDirection.Ascending.ToString();

        try
        {
            var result = await GetNewMediator().Send(Query, cancellationToken);

            if (result is { Succeeded: true, Data: not null })
            {
                _data = result.Data;
                return new TableData<ParticipantEngagementDto>
                {
                    TotalItems = _data.Details.TotalItems,
                    Items = _data.Details.Items
                };
            }

            _data = null;
            if (result?.ErrorMessage is not null)
            {
                Snackbar.Add(result.ErrorMessage, Severity.Warning);
            }
        }
        catch (OperationCanceledException)
        {
            // Component disposed or navigated away; nothing to do.
        }

        return new TableData<ParticipantEngagementDto> { TotalItems = 0, Items = [] };
    }

    private async Task OnHideRecentChanged(bool value)
    {
        Query.HideRecentEngagements = value;
        await OnRefresh();
    }

    private async Task OnLocationChanged(int locationId)
    {
        _selectedLocationId = locationId;
        Query.LocationId = locationId == 0 ? null : locationId;
        await OnRefresh();
    }

    private async Task ShowLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(CurrentUser);

        if (location is not null)
        {
            _locations[location.Id] = location.Name;
            await OnLocationChanged(location.Id);
        }
    }

    private async Task OnEngagementTypeChanged(string engagementType)
    {
        _selectedEngagementType = engagementType ?? string.Empty;
        Query.EngagementType = string.IsNullOrEmpty(engagementType) ? null : engagementType;
        await OnRefresh();
    }

    private async Task ShowTenantDialog()
    {
        var tenant = await ParticipantDialogService.PromptForTenantAsync(CurrentUser);

        if (tenant is not null)
        {
            Query.TenantId = tenant.TenantId;
            _selectedTenantName = tenant.DisplayName;
            await OnRefresh();
        }
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var exportQuery = new GetParticipantsLatestEngagement.Query
            {
                CurrentUser = CurrentUser,
                JustMyCases = Query.JustMyCases,
                HideRecentEngagements = Query.HideRecentEngagements,
                LocationId = Query.LocationId,
                EngagementType = Query.EngagementType,
                TenantId = Query.TenantId,
                OrderBy = "EngagedOn",
                SortDirection = SortDirection.Descending.ToString()
            };

            var result = await GetNewMediator().Send(new ExportParticipantsLatestEngagement.Command
            {
                Query = exportQuery
            });

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add("An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task ClearSearch()
    {
        _selectedEngagementType = string.Empty;
        _selectedEngagementType = string.Empty;
        _selectedLocationId = 0;
        _selectedTenantName = null;
        
        Query.TenantId = null;
        Query.LocationId = null;
        Query.EngagementType = null;
        Query.HideRecentEngagements = false;

        await OnRefresh();
    }
}
