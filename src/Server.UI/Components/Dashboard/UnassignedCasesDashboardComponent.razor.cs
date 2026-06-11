using ApexCharts;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Application.Common.Security;
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

    /// <summary>
    /// Creates a UserProfile with the selected TenantId for querying data
    /// </summary>
    private UserProfile GetEffectiveUserProfile()
    {
        // If TenantId parameter matches CurrentUser's TenantId, use it as-is
        if (TenantId == CurrentUser.TenantId)
        {
            return CurrentUser;
        }

        // Create a copy with the selected TenantId
        return new UserProfile
        {
            UserId = CurrentUser.UserId,
            UserName = CurrentUser.UserName,
            Email = CurrentUser.Email,
            DisplayName = CurrentUser.DisplayName,
            PhoneNumber = CurrentUser.PhoneNumber,
            TenantId = TenantId,
            TenantName = CurrentUser.TenantName,
            AssignedRoles = CurrentUser.AssignedRoles,
            DefaultRole = CurrentUser.DefaultRole,
            Contracts = CurrentUser.Contracts,
            IsActive = CurrentUser.IsActive,
            Provider = CurrentUser.Provider,
            SuperiorName = CurrentUser.SuperiorName,
            SuperiorId = CurrentUser.SuperiorId,
            ProfilePictureDataUrl = CurrentUser.ProfilePictureDataUrl
        };
    }

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.AuthorizedUser)).Succeeded;
        _canReassign = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Reassign)).Succeeded;
        _previousVisualMode = VisualMode;
        _previousTenantId = TenantId;

        _locations = LocationService.GetVisibleLocations(TenantId)
                        .ToDictionary(k => k.Id, e => e.Name);

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

    protected override IRequest<Result<GetUnassignedCasesSummary.UnassignedCasesSummaryDto>> CreateQuery()
        => new GetUnassignedCasesSummary.Query
        {
            CurrentUser = GetEffectiveUserProfile()
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
            Query.CurrentUser = GetEffectiveUserProfile();
            Query.OrderBy = state.SortDefinitions.FirstOrDefault()?.SortBy ?? "LastModified";
            Query.SortDirection = state.SortDefinitions.FirstOrDefault()?.Descending ?? true 
                ? SortDirection.Descending.ToString() 
                : SortDirection.Ascending.ToString();
            Query.PageNumber = state.Page + 1;
            Query.PageSize = state.PageSize;
            Query.IncludeTransferIn = _includeTransferIn;

            var result = await Service.Send(Query, cancellationToken);

            return new GridData<UnassignedCaseDto>()
            {
                Items = result.Data!.Items,
                TotalItems = result.Data.TotalItems
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
        await _table.ReloadServerData();
    }

    private async Task EnrolmentStatusChanged(int? statusValue)
    {
        Query.EnrolmentStatus = statusValue;
        await _table.ReloadServerData();
    }

    private async Task ShowSelectLocationDialog()
    {
        var parameters = new DialogParameters<SelectLocationDialog>
        {
            { "CurrentUser", GetEffectiveUserProfile() }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectLocationDialog>("Select a location", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: LocationDto location })
        {
            Query.Locations = [location.Id];
            await _table.ReloadServerData();
        }
    }

    private async Task ClearFilters()
    {
        Query.Keyword = null;
        Query.EnrolmentStatus = null;
        Query.Locations = [];
        Query.IncludeTransferIn = _includeTransferIn;
        await _table.ReloadServerData();
    }

    private async Task OnIncludeTransferInChanged(bool value)
    {
        _includeTransferIn = value;
        Query.IncludeTransferIn = value;
        await _table.ReloadServerData();
    }

    private async Task AssignToSelf(UnassignedCaseDto participant)
    {
        var effectiveUser = GetEffectiveUserProfile();

        var command = new AssignUnassignedCase.Command
        {
            ParticipantId = participant.Id,
            CurrentUser = effectiveUser,
            AssigneeId = effectiveUser.UserId
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
        var effectiveUser = GetEffectiveUserProfile();

        var parameters = new DialogParameters<AssignCaseDialog>
        {
            {
                x => x.Participant,
                participant
            },
            {
                x => x.TenantId,
                TenantId
            },
            {
                x => x.Model,
                new AssignUnassignedCase.Command
                {
                    ParticipantId = participant.Id,
                    CurrentUser = effectiveUser
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
        Navigation.NavigateTo($"/pages/participants/{participantId}");

    private void NavigateToTransfers() =>
        Navigation.NavigateTo("/pages/participants/transfers");
}

public record ChartDataPoint(string LocationName, EnrolmentStatus Status, int Count);
