using ApexCharts;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
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

    [CascadingParameter(Name = "IsDarkMode")]
    public bool IsDarkMode { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    public UserProfile? UserProfile { get; set; }

    private int _defaultPageSize = 15;
    private MudDataGrid<UnassignedCaseDto> _table = default!;
    private bool _loading;
    private bool _canSearch;
    private List<ChartDataPoint>? _chartData;
    private bool _previousVisualMode;
    private GetUnassignedCasesSummary.UnassignedCasesSummaryDto? _summaryData;

    private UnassignedCasesWithPagination.Query Query { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.AuthorizedUser)).Succeeded;
        _previousVisualMode = VisualMode;
        
        if (VisualMode && UserProfile != null)
        {
            await LoadChartData();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload chart data when switching to visual mode
        if (VisualMode && (!_previousVisualMode || _chartData == null) && UserProfile != null)
        {
            await LoadChartData();
        }
        _previousVisualMode = VisualMode;
    }

    private async Task LoadChartData()
    {
        try
        {
            _loading = true;
            
            var query = new GetUnassignedCasesSummary.Query
            {
                CurrentUser = UserProfile!
            };

            var result = await GetNewMediator().Send(query);
            
            if (result.Succeeded && result.Data != null && result.Data.Summaries.Any())
            {
                _summaryData = result.Data;
                _chartData = result.Data.Summaries
                    .Select(s => new ChartDataPoint(s.LocationName, s.EnrolmentStatus, s.Count))
                    .ToList();
            }
            else
            {
                _summaryData = null;
                _chartData = new List<ChartDataPoint>();
            }
        }
        finally
        {
            _loading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private List<ChartDataPoint> ChartData()
    {
        if (_chartData is null || !_chartData.Any())
        {
            return new List<ChartDataPoint>();
        }

        var locations = _chartData.Select(x => x.LocationName).Distinct().ToList();
        var statuses = _chartData.Select(x => x.Status).Distinct().ToList();

        return locations.SelectMany(location =>
            statuses.Select(status => new ChartDataPoint(
                location,
                status,
                _chartData
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
            _loading = true;
            Query.CurrentUser = UserProfile;
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "LastModified";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true 
                ? SortDirection.Descending.ToString() 
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;

            var result = await GetNewMediator().Send(Query, cancellationToken);

            return new GridData<UnassignedCaseDto>()
            {
                Items = result.Data!.Items,
                TotalItems = result.Data.TotalItems
            };
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await _table.ReloadServerData();
    }

    private async Task EnrolmentStatusChanged(int? statusValue)
    {
        Query.EnrolmentStatus = statusValue;
        await _table.ReloadServerData();
    }

    private async Task AssignToSelf(UnassignedCaseDto participant)
    {
        var command = new AssignUnassignedCase.Command
        {
            ParticipantId = participant.Id,
            CurrentUser = UserProfile,
            AssigneeId = UserProfile!.UserId
        };

        var result = await GetNewMediator().Send(command);
        
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
        var parameters = new DialogParameters
        {
            { "Participant", participant },
            { "TenantId", UserProfile!.TenantId },
            { "CurrentUser", UserProfile }
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
        Navigation.NavigateTo($"/pages/participants/{participantId}");

    private void NavigateToTransfers() =>
        Navigation.NavigateTo("/pages/participants/transfers");
}

public record ChartDataPoint(string LocationName, EnrolmentStatus Status, int Count);
