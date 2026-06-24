using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Components.Locations;
using Cfo.Cats.Server.UI.Pages.Activities;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public enum ActivitiesQuickFilter
{
    All,
    MyActivities,
    ReturnedToMe,
    ReturnedLast7Days,
    ReturnedLast14Days,
    ReturnedLast30Days
}

public partial class Activities
{
    [Inject]
    public ActivitiesSessionStorage SessionStorage { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;

    [Inject]
    public ITenantService TenantService { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private ActivityPaginationDto[] _data = [];
    private int _totalPages;
    private int _totalItems;

    private IDictionary<string, string> _users = new Dictionary<string, string>();
    private IDictionary<string, string> _tenants = new Dictionary<string, string>();

    private ActivitiesQuickFilter _currentFilter = ActivitiesQuickFilter.All;

    private bool Tabular { get; set; }

    private bool _downloading;

    private readonly HashSet<Guid> _expandedRows = [];

    private AllActivitiesWithPagination.Query Query { get; set; } = new()
    {
        UserProfile = null!,
        PageNumber = 1,
        PageSize = 10,
        OrderBy = "Created",
        SortDirection = "Descending"
    };

    private void OnRowClick(TableRowClickEventArgs<ActivityPaginationDto> args)
    {
        if(args?.Item is not null)
        {
            ViewParticipant(args.Item.ParticipantId);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Query.UserProfile = UserProfile;

        var state = await AuthState;
        Query.IncludeInternalNotes = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.Internal)).Succeeded;

        _users = UserService.DataSource
            .Where(d => d.TenantId!.StartsWith(UserProfile.TenantId!))
            .ToDictionary(a => a.Id, e => e.DisplayName);

        _tenants = TenantService.GetVisibleTenants(UserProfile.TenantId!)
            .ToDictionary(k => k.Id, k => k.Name);

        var cached = await SessionStorage.GetAsync();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            Query.TenantId = sd.TenantId;
            Query.OwnerId = sd.OwnerId;
            Query.LocationId = sd.LocationId;
            Query.LocationName = sd.LocationName;
            Query.Status = sd.Status;
            Query.TypeFilter = sd.TypeFilter;
            Query.ReturnedWithinDays = sd.ReturnedWithinDays;
            Query.Keyword = sd.Keyword;
            Query.OrderBy = sd.OrderBy ?? "Created";
            Query.SortDirection = sd.SortDirection ?? "Descending";
            Query.PageNumber = sd.PageNumber;
            Tabular = sd.Tabular;

            _currentFilter = ResolveQuickFilter(sd);
        }

        await OnRefresh();
    }

    private ActivitiesQuickFilter ResolveQuickFilter(ActivitiesSessionData sd)
        => sd.ReturnedWithinDays switch
        {
            7 => ActivitiesQuickFilter.ReturnedLast7Days,
            14 => ActivitiesQuickFilter.ReturnedLast14Days,
            30 => ActivitiesQuickFilter.ReturnedLast30Days,
            _ when sd.OwnerId == UserProfile.UserId && sd.Status == ActivityStatus.PendingStatus.Value => ActivitiesQuickFilter.ReturnedToMe,
            _ when sd.OwnerId == UserProfile.UserId => ActivitiesQuickFilter.MyActivities,
            _ => ActivitiesQuickFilter.All
        };

    private async Task OnRefresh()
    {
        Query.UserProfile = UserProfile;
        var result = await Service.Send(Query);

        if (result is { Succeeded: true, Data: not null })
        {
            _data = result.Data.Items.ToArray();
            _totalPages = result.Data.TotalPages;
            _totalItems = result.Data.TotalItems;
        }
        else
        {
            _data = [];
            _totalPages = 0;
            _totalItems = 0;
        }

        await SessionStorage.SetAsync(ActivitiesSessionData.FromQuery(Query, Tabular));
    }

    private async Task TabularChanged(bool? tabular)
    {
        Tabular = tabular.GetValueOrDefault();
        await OnRefresh();
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            Query.UserProfile = UserProfile;

            var result = await Service.Send(new ExportActivities.Command()
            {
                Query = Query
            });

            if (result.Succeeded)
            {
                Snackbar.Add($"{ConstantString.ExportSuccess}", Severity.Info);
                return;
            }

            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
        catch
        {
            Snackbar.Add($"An error occurred while generating your document.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private bool IsExpanded(Guid activityId) => _expandedRows.Contains(activityId);

    private void ToggleExpanded(Guid activityId)
    {
        if (!_expandedRows.Add(activityId))
        {
            _expandedRows.Remove(activityId);
        }
    }

    private async Task PageChanged(int page)
    {
        Query.PageNumber = page;
        await OnRefresh();
    }

    private async Task OnSearch(string? keyword)
    {
        Query.Keyword = keyword;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task SortBy(string key)
    {
        if (Query.OrderBy == key)
        {
            Query.SortDirection = Query.SortDirection == "Ascending" ? "Descending" : "Ascending";
        }
        else
        {
            Query.OrderBy = key;
            Query.SortDirection = SortDirection.Ascending.ToString();
        }
        await OnRefresh();
    }

    private async Task StatusChanged(ActivityStatus? status)
    {
        Query.Status = status?.Value;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task ActivityTypeChanged(ActivityType? type)
    {
        Query.TypeFilter = type?.Value;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task ShowSubmittedByDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog> { { "CurrentUser", UserProfile } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select a user", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            Query.OwnerId = user.UserId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ShowTenantDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog> { { "CurrentUser", UserProfile } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select a tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            Query.TenantId = tenant.TenantId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ShowSelectLocationDialog()
    {
        var parameters = new DialogParameters<SelectLocationDialog> { { "CurrentUser", UserProfile } };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectLocationDialog>("Select a location", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: LocationDto location })
        {
            Query.LocationId = location.Id;
            Query.LocationName = location.Name;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ApplyMyActivitiesFilter()
    {
        ResetQuery();
        _currentFilter = ActivitiesQuickFilter.MyActivities;
        Query.OwnerId = UserProfile.UserId;
        await OnRefresh();
    }

    private async Task ApplyReturnedToMeFilter()
    {
        ResetQuery();
        _currentFilter = ActivitiesQuickFilter.ReturnedToMe;
        Query.OwnerId = UserProfile.UserId;
        Query.Status = ActivityStatus.PendingStatus.Value;
        await OnRefresh();
    }

    private async Task ApplyReturnedWithinFilter(int days)
    {
        ResetQuery();
        _currentFilter = days switch
        {
            7 => ActivitiesQuickFilter.ReturnedLast7Days,
            14 => ActivitiesQuickFilter.ReturnedLast14Days,
            30 => ActivitiesQuickFilter.ReturnedLast30Days,
            _ => ActivitiesQuickFilter.All
        };
        Query.ReturnedWithinDays = days;
        await OnRefresh();
    }

    private async Task ClearQuickFilter()
    {
        ResetQuery();
        _currentFilter = ActivitiesQuickFilter.All;
        await OnRefresh();
    }

    private async Task ClearSearch()
    {
        ResetQuery();
        _currentFilter = ActivitiesQuickFilter.All;
        await OnRefresh();
    }

    private void ResetQuery()
    {
        Query.TenantId = null;
        Query.OwnerId = null;
        Query.LocationId = null;
        Query.LocationName = null;
        Query.Status = null;
        Query.TypeFilter = null;
        Query.ReturnedWithinDays = null;
        Query.Keyword = null;
        Query.OrderBy = "Created";
        Query.SortDirection = SortDirection.Descending.ToString();
        Query.PageNumber = 1;
        Tabular = false;
    }

    private string GetCurrentFilterLabel()
        => _currentFilter switch
        {
            ActivitiesQuickFilter.MyActivities => "My Activities",
            ActivitiesQuickFilter.ReturnedToMe => "Returned to me",
            ActivitiesQuickFilter.ReturnedLast7Days => "Returns (Last 7 Days)",
            ActivitiesQuickFilter.ReturnedLast14Days => "Returns (Last 14 Days)",
            ActivitiesQuickFilter.ReturnedLast30Days => "Returns (Last 30 Days)",
            _ => "All"
        };

    private async Task EditActivity(ActivityPaginationDto activity)
    {
        var parameters = new DialogParameters<EditActivityDialog>
        {
            { x => x.ActivityId, activity.ActivityId }
        };

        var dialog = await DialogService.ShowAsync<EditActivityDialog>("Edit Activity/ETE", parameters, new DialogOptions
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        });

        var result = await dialog.Result;

        if (result is { Canceled: false })
        {
            await OnRefresh();
        }
    }

    private void ViewParticipant(string participantId)
        => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}");

}
