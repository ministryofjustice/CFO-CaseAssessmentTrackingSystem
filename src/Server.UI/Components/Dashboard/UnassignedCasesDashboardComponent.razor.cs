using ApexCharts;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Server.UI.Components.Locations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Cfo.Cats.Server.UI.Components.Dashboard;

public partial class UnassignedCasesDashboardComponent
{
    [Parameter]
    public string TenantId { get; set; } = default!;

    [EditorRequired, Parameter]
    public bool VisualMode { get; set; }

    [Parameter]
    public bool IncludeTransferIn { get; set; } = true;

    [Parameter]
    public string? KeywordFilter { get; set; }

    [Parameter]
    public int? SelectedEnrolmentStatus { get; set; }

    [Parameter]
    public int? SelectedLocationId { get; set; }

    [Parameter]
    public EventCallback<UnassignedCasesDashboardFilters> FiltersChanged { get; set; }

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    public ILocationService LocationService { get; set; } = default!;

    private int _defaultPageSize = 15;
    private MudDataGrid<UnassignedCaseDto> _table = default!;
    private bool _canSearch;
    private bool _canReassign;
    
    private bool _previousVisualMode;
    private string? _previousTenantId;
    private IDictionary<int, string> _locations = null!;
    private bool _includeTransferIn = true;

    private UnassignedCasesWithPagination.Query Query { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, null, SecurityPolicies.AuthorizedUser)).Succeeded;
        _canReassign = (await AuthService.AuthorizeAsync(state.User, null, SecurityPolicies.Reassign)).Succeeded;
        _previousVisualMode = VisualMode;
        _previousTenantId = TenantId;

        _locations = LocationService.GetVisibleLocations(TenantId)
                        .ToDictionary(k => k.Id, e => e.Name);

        _includeTransferIn = IncludeTransferIn;
        Query.IncludeTransferIn = _includeTransferIn;
        Query.Keyword = KeywordFilter;
        Query.EnrolmentStatus = SelectedEnrolmentStatus;
        Query.Locations = SelectedLocationId.HasValue ? [SelectedLocationId.Value] : [];

        if (EnsureValidLocationFilter())
        {
            await NotifyFiltersChanged();
        }

        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        var tenantChanged = _previousTenantId != TenantId;

        // Reload locations if tenant changed
        if (tenantChanged)
        {
            _locations = LocationService.GetVisibleLocations(TenantId)
                .ToDictionary(k => k.Id, e => e.Name);
            _previousTenantId = TenantId;

            if (EnsureValidLocationFilter())
            {
                await NotifyFiltersChanged();
            }
        }

        // Reload summary data when switching to visual mode or tenant changed
        if (VisualMode && (tenantChanged || !_previousVisualMode))
        {
            await LoadDataAsync();
        }

        // Reload table data if tenant changed and in tabular mode
        if (!VisualMode && tenantChanged && _table != null)
        {
            await _table.ReloadServerData();
        }

        _previousVisualMode = VisualMode;
    }

    protected override IQuery<Result<GetUnassignedCasesSummary.UnassignedCasesSummaryDto>> CreateQuery()
        => new GetUnassignedCasesSummary.Query
        {
            CurrentUser = CurrentUser,
            TenantId = TenantId
        };

    private List<ChartDataPoint> ChartData()
    {
        var chartSource = Data?.Summaries
            .Select(s => new ChartDataPoint(s.LocationName, s.EnrolmentStatus, s.Count))
            .ToList() ?? [];

        if (!chartSource.Any())
        {
            return [];
        }

        var locations = chartSource.Select(x => x.LocationName).Distinct().ToList();
        var statuses = chartSource.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new ChartDataPoint(
                location,
                status,
                chartSource
                    .Where(x => x.LocationName == location && x.Status == status)
                    .Sum(x => x.Count))))
            .ToList();
    }

    private ApexChartOptions<ChartDataPoint> ChartOptions => new()
    {
        Chart = new Chart
        {
            Stacked = true,
        },
        
        Legend = new Legend
        {
            Show = true,
            ShowForSingleSeries = true
        },
        
        PlotOptions = new PlotOptions
        {
            Bar = new PlotOptionsBar
            {
                DataLabels = new PlotOptionsBarDataLabels
                {
                    Total = new BarTotalDataLabels
                    {
                        Style = new BarDataLabelsStyle
                        {
                            FontWeight = "800",
                            Color = IsDarkMode ? "#FFFFFF" : "#000000",
                        }
                    }
                },
            },
        },
        Yaxis =
        [
            new YAxis
            {
                Min = 0,
                ForceNiceScale = true
            }
        ],
        Theme = new Theme
        {
            Mode = IsDarkMode ? Mode.Dark : Mode.Light
        }
    };

    private async Task<GridData<UnassignedCaseDto>> ServerReload(GridState<UnassignedCaseDto> state, CancellationToken cancellationToken)
    {
        try
        {
            Loading = true;
            Query.CurrentUser = CurrentUser;
            Query.TenantId = TenantId;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "LastModified";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true 
                ? SortDirection.Descending.ToString() 
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            Query.IncludeTransferIn = _includeTransferIn;

            var result = await Service.Send(Query, cancellationToken);

            if (result is not { Succeeded: true, Data: { } data })
            {
                ErrorMessage = result?.ErrorMessage ?? "Unable to load unassigned cases.";
                return new GridData<UnassignedCaseDto>
                {
                    Items = [],
                    TotalItems = 0
                };
            }

            return new GridData<UnassignedCaseDto>()
            {
                Items = data.Items,
                TotalItems = data.TotalItems
            };
        }
        finally
        {
            Loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await NotifyFiltersChanged();
        await _table.ReloadServerData();
    }

    private async Task EnrolmentStatusChanged(int? statusValue)
    {
        Query.EnrolmentStatus = statusValue;
        await NotifyFiltersChanged();
        await _table.ReloadServerData();
    }

    private async Task ShowSelectLocationDialog()
    {
        var parameters = new DialogParameters<SelectLocationDialog>
        {
            { "CurrentUser", CurrentUser },
            { "TenantId", TenantId }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectLocationDialog>("Select a location", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: LocationDto location })
        {
            Query.Locations = [location.Id];
            await NotifyFiltersChanged();
            await _table.ReloadServerData();
        }
    }

    private async Task ClearFilters()
    {
        Query.Keyword = null;
        Query.EnrolmentStatus = null;
        Query.Locations = [];
        Query.IncludeTransferIn = _includeTransferIn;
        await NotifyFiltersChanged();
        await _table.ReloadServerData();
    }

    private async Task OnIncludeTransferInChanged(bool value)
    {
        _includeTransferIn = value;
        Query.IncludeTransferIn = value;
        await NotifyFiltersChanged();
        await _table.ReloadServerData();
    }

    private Task NotifyFiltersChanged()
    {
        if (FiltersChanged.HasDelegate is false)
        {
            return Task.CompletedTask;
        }

        return FiltersChanged.InvokeAsync(new UnassignedCasesDashboardFilters
        {
            IncludeTransferIn = _includeTransferIn,
            Keyword = Query.Keyword,
            EnrolmentStatus = Query.EnrolmentStatus,
            LocationId = Query.Locations is { Length: > 0 } ? Query.Locations[0] : null
        });
    }

    private bool EnsureValidLocationFilter()
    {
        if (Query.Locations is not { Length: > 0 })
        {
            return false;
        }

        if (_locations.ContainsKey(Query.Locations[0]))
        {
            return false;
        }

        Query.Locations = [];
        return true;
    }

    private string GetSelectedLocationLabel()
    {
        if (Query.Locations is not { Length: > 0 })
        {
            return "All Locations";
        }

        return _locations.TryGetValue(Query.Locations[0], out var locationName)
            ? locationName
            : "All Locations";
    }

    private async Task AssignToSelf(UnassignedCaseDto participant)
    {
        var command = new AssignUnassignedCase.Command
        {
            ParticipantId = participant.Id,
            CurrentUser = CurrentUser,
            AssigneeId = CurrentUser.UserId
        };

        var result = await Service.Send(command);
        
        if (result.Succeeded)
        {
            Snackbar.Add($"Case {participant.ParticipantName} assigned to you", Severity.Success);
            await _table.ReloadServerData();
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }

    private async Task AssignToOther(UnassignedCaseDto participant)
    {
        var parameters = new DialogParameters<AssignCaseDialog>
        {
            {
                x => x.Participant,
                participant
            },
            {
                x => x.TenantId,
                CurrentUser.TenantId!
            },
            {
                x => x.Model,
                new AssignUnassignedCase.Command
                {
                    ParticipantId = participant.Id,
                    CurrentUser = CurrentUser
                }
            }
        };

        var options = new DialogOptions 
        { 
            CloseButton = true, 
            MaxWidth = MaxWidth.Medium, 
            FullWidth = true 
        };

        var dialog = await DialogService.ShowAsync<AssignCaseDialog>(
            $"Assign {participant.ParticipantName}", 
            parameters, 
            options);
        
        var state = await dialog.Result;

        if (state?.Canceled == false)
        {
            await _table.ReloadServerData();
        }
    }

    private void ViewParticipant(string participantId) =>
        Navigation.NavigateTo($"/pages/workspace/participants/{participantId}?from=unassigned-cases");

    private void NavigateToTransfers() =>
        Navigation.NavigateTo("/pages/workspace/participants/transfers");
}

public record ChartDataPoint(string LocationName, EnrolmentStatus Status, int Count);

public record UnassignedCasesDashboardFilters
{
    public bool IncludeTransferIn { get; init; } = true;
    public string? Keyword { get; init; }
    public int? EnrolmentStatus { get; init; }
    public int? LocationId { get; init; }
}
