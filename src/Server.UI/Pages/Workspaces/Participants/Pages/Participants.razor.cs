using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Components.Locations;
using Cfo.Cats.Server.UI.Pages.Participants.Components;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public enum QuickFilter
{
    All,
    MyParticipants,
    OverdueRisk,
    AssignedLast10Days,
    AssignedLast30Days,
    VisitedLast7Days
}

public partial class Participants
{

     [Inject]
    public ILocationService LocationService { get; set; } = null!;

    [Inject]
    public IUserService UserService {get;set;} = null!;

    [Inject]
    public ITenantService TenantService {get;set;} = null!;

    [Inject]
    public ParticipantsSessionStorage SessionStorage { get;set; } = null!;

    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = default!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private IDictionary<int, string> _locations = null!;
    private IDictionary<string, string> _users = null!;

    private IDictionary<string, string> _tenants = null!;

    private IDictionary<Guid, LabelDto> _labels = null!;
    private int _totalPages = 0;
    private int _totalItems = 0;

    private bool Tabular { get; set; } = false;

    private const int DefaultPageSize = 50;
    private const int GroupPageSize = 10;

    private ParticipantGroupView[] _groups = [];

    private bool _downloading = false;
    private bool _canReassign;
    private HashSet<string> _selectedParticipantIds = [];
    private QuickFilter _currentFilter = QuickFilter.All;

    private ParticipantPaginationDto[] _data = [];

    private ParticipantsWithPagination.Query Query { get; set; } = new ParticipantsWithPagination.Query()
    {
        JustMyCases = false,
        ListView = ParticipantListView.Default,
        PageNumber = 1,
        PageSize = 50,
        Keyword = null

    };

    protected override async Task OnInitializedAsync()
    {
        _locations = LocationService.GetVisibleLocations(UserProfile.TenantId!)
                        .ToDictionary( k => k.Id, e => e.Name );

        var authState = await AuthState;
        _canReassign = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Reassign)).Succeeded;

        _users = UserService.DataSource
            .Where(d => d.TenantId!.StartsWith(UserProfile.TenantId!))
            .ToDictionary(a => a.Id, e => e.DisplayName);

        _tenants = TenantService.GetVisibleTenants(UserProfile.TenantId!)
                    .ToDictionary(k => k.Id, k => k.Name);

        var labelsResult = await Service.Send(new GetVisibleLabels.Query(UserProfile!));

        if(labelsResult.Succeeded && labelsResult.Data is not null)
        {
            _labels = labelsResult.Data
                        .ToDictionary(k => k.Id, v => v);

        }

        if(UserProfile.AssignedRoles is [])
        {
            // support workers default to MY participants
            // if I have a cached query, this will be overriden below
            Query.OwnerId = UserProfile.UserId;
            _currentFilter = QuickFilter.MyParticipants;
        }
                    
        var cached = await SessionStorage.GetAsync();

        if (cached is { Succeeded: true, Data: { } sd })
        {
            Query.Keyword = sd.Keyword;
            Query.Label = sd.LabelId;
            Query.ListView = sd.ListView;
            Query.Locations = sd.Locations;
            Query.OrderBy = sd.OrderBy ?? "Id";
            Query.SortDirection = sd.SortDirection;
            Query.PageNumber = sd.PageNumber;
            Query.OwnerId = sd.OwnerId;
            Query.TenantId = sd.TenantId;
            Query.RiskDue = sd.RiskDue;
            Query.RecentAction = sd.RecentlyAssigned;
            Query.GroupBy = sd.GroupBy;
            Tabular = sd.Tabular;
            
            // Sync _currentFilter based on restored state
            _currentFilter = sd.RecentlyAssigned switch
            {
                RecentParticipantFilter.AssignedLast10Days => QuickFilter.AssignedLast10Days,
                RecentParticipantFilter.AssignedLast30Days => QuickFilter.AssignedLast30Days,
                RecentParticipantFilter.VisitedLast7Days => QuickFilter.VisitedLast7Days,
                _ when sd.RiskDue.HasValue => QuickFilter.OverdueRisk,
                _ when !string.IsNullOrEmpty(sd.OwnerId) => QuickFilter.MyParticipants,
                _ => QuickFilter.All
            };
        }

        await OnRefresh();
    }

    private async Task LocationValueChanged(int? locationId)
    {
        Query.Locations = locationId == null ? [] : [locationId.Value];
        await OnRefresh();
    }

    private async Task TabularChanged(bool? tabular)
    {
        Tabular = tabular.GetValueOrDefault();
        await OnRefresh();
    }

    private async Task LabelsValueChanged(LabelDto? labelDto)
    {
        Query.Label = labelDto is not null ? new LabelId(labelDto.Id) : null;
        await OnRefresh();
    }

    private async Task RecentlyAssignedFilterChanged(RecentParticipantFilter filter)
    {
        // Reset query like other quick filters to avoid unintended filter combinations
        ResetQuery();
        
        Query.RecentAction = filter;
        
        // Update the current filter tracking
        _currentFilter = filter switch
        {
            RecentParticipantFilter.AssignedLast10Days => QuickFilter.AssignedLast10Days,
            RecentParticipantFilter.AssignedLast30Days => QuickFilter.AssignedLast30Days,
            _ => QuickFilter.All
        };
        
        await OnRefresh();
    }

    private async Task RecentlyVisitedFilterChanged()
    {
        // Reset query like other quick filters to avoid unintended filter combinations
        ResetQuery();
        
        Query.RecentAction = RecentParticipantFilter.VisitedLast7Days;
        // Update the current filter tracking
        _currentFilter = QuickFilter.VisitedLast7Days;
        
        await OnRefresh();
    }
    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        await OnRefresh();
    }

    private async Task ListViewChanged(ParticipantListView listView)
    {
        Query.ListView = listView;
        await OnRefresh();
    }

    private async Task PageChanged(int page)
    {
        Query.PageNumber = page;
        await OnRefresh();
    }

    private async Task ClearSearch()
    {
        ResetQuery();
        _currentFilter = QuickFilter.All;
        await OnRefresh();
    }

    private async Task OnRefreshClicked()
    {
        _selectedParticipantIds.Clear();
        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;

        if (Query.GroupBy == ParticipantGroupBy.None)
        {
            Query.PageSize = DefaultPageSize;

            var results = await Service.Send(Query);
            if (results is { Succeeded: true, Data: not null })
            {
                _data = results.Data.Items.ToArray();
                _totalPages = results.Data.TotalPages;
                _totalItems = results.Data.TotalItems;
            }
            else
            {
                _data = [];
                _totalPages = 0;
                _totalItems = 0;
            }

            _groups = [];
        }
        else
        {
            // Grouped mode: the pager pages through the groups (e.g. assignees); each group
            // carries all of its participant rows.
            var grouped = await Service.Send(new GetGroupedParticipants.Query
            {
                Filter = Query,
                GroupPageNumber = Query.PageNumber,
                GroupPageSize = GroupPageSize
            });

            if (grouped is { Succeeded: true, Data: not null })
            {
                _groups = grouped.Data.Groups.Select(g => new ParticipantGroupView(g)).ToArray();
                _totalItems = grouped.Data.TotalGroups;
                _totalPages = (int)Math.Ceiling(grouped.Data.TotalGroups / (double)GroupPageSize);
            }
            else
            {
                _groups = [];
                _totalItems = 0;
                _totalPages = 0;
            }

            _data = [];
        }

        await SessionStorage.SetAsync(ParticipantsSessionData.FromQuery(Query, Tabular));
    }

    private async Task GroupByChanged(ParticipantGroupBy groupBy)
    {
        Query.GroupBy = groupBy;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task OnGroupExpanded(ParticipantGroupView group, bool expanded)
    {
        if (!expanded || group.Loaded || group.Loading)
        {
            return;
        }

        group.Loading = true;

        var result = await Service.Send(BuildGroupQuery(group.Key));
        group.Items = result is { Succeeded: true, Data: not null } ? result.Data.Items.ToArray() : [];
        group.Loaded = true;
        group.Loading = false;
    }

    // Fetches a single group's participants by reusing the flat query, scoped to the group key.
    private ParticipantsWithPagination.Query BuildGroupQuery(string key) => new()
    {
        CurrentUser = UserProfile,
        ListView = Query.ListView,
        JustMyCases = Query.JustMyCases,
        Locations = Query.Locations,
        Label = Query.Label,
        Keyword = Query.Keyword,
        RiskDue = Query.RiskDue,
        RecentAction = Query.RecentAction,
        OrderBy = Query.OrderBy,
        SortDirection = Query.SortDirection,
        GroupBy = ParticipantGroupBy.None,
        OwnerId = Query.GroupBy == ParticipantGroupBy.Assignee ? key : Query.OwnerId,
        TenantId = Query.GroupBy == ParticipantGroupBy.Tenant ? key : Query.TenantId,
        PageNumber = 1,
        PageSize = DefaultPageSize
    };

    private void ClearSelectedParticipants() => _selectedParticipantIds.Clear();

    private async Task ReassignSelectedItems()
    {
        if (_selectedParticipantIds.Count == 0)
        {
            return;
        }

        var parameters = new DialogParameters<ReassignParticipantDialog>
        {
            {
                x => x.Model, new ReassignParticipants.Command()
                {
                    CurrentUser = UserProfile,
                    ParticipantIdsToReassign = _selectedParticipantIds.ToArray()
                }
            },
            {
                x => x.UserProfile,
                UserProfile
            }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<ReassignParticipantDialog>("Reassign participants", parameters, options);
        var state = await dialog.Result;

        if (state?.Canceled == false)
        {
            ParticipantCacheKey.Refresh();
            _selectedParticipantIds.Clear();
            await OnRefresh();
        }
    }

    private async Task ShowSelectLocationDialog()
    {
        var parameters = new DialogParameters<SelectLocationDialog>
        {
            { "CurrentUser", UserProfile }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectLocationDialog>("Select a location", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: LocationDto location })
        {
            Query.Locations = [location.Id];
            await OnRefresh();
        }
    }

    private async Task ShowAssigneeDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", UserProfile }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select an assingee", parameters, options);
        var result = await dialog.Result;

        if(result is { Canceled: false, Data: SelectedUser user })
        {
            Query.OwnerId = user.UserId;
            await OnRefresh();
        }
    }

    private async Task ShowTenantDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog>()
        {
            { "CurrentUser", UserProfile }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select a tenant", parameters, options);
        var result = await dialog.Result;

        if(result is { Canceled: false, Data: SelectedTenant tenant })
        {
            Query.TenantId = tenant.TenantId;
            await OnRefresh();
        }
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

    private async Task ApplyMyParticipantsFilter()
    {
        ResetQuery();
        _currentFilter = QuickFilter.MyParticipants;
        Query.OwnerId = UserProfile.UserId;
        await OnRefresh();
    }

    private async Task ApplyOverdueRiskFilter()
    {
        ResetQuery();
        _currentFilter = QuickFilter.OverdueRisk;
        Query.RiskDue = DateTime.UtcNow.Date;
        Query.SortDirection = SortDirection.Ascending.ToString();
        Query.OrderBy = "RiskDue";
        await OnRefresh();
    }

    private async Task ClearQuickFilter()
    {
        ResetQuery();
        _currentFilter = QuickFilter.All;
        await OnRefresh();
    }

    private void ResetQuery()
    {
        Query.Locations = [];
        Query.TenantId = null;
        Query.Keyword = null;
        Query.OrderBy = "Id";
        Query.ListView = ParticipantListView.Default;
        Query.SortDirection = SortDirection.Ascending.ToString();
        Query.PageNumber = 1;
        Query.Label = null;
        Query.OwnerId = null;
        Query.RiskDue = null;
        Query.RecentAction = RecentParticipantFilter.All;
        Query.JustMyCases = false;
        Query.GroupBy = ParticipantGroupBy.None;
        Tabular = false;
    }

    private string GetCurrentFilterLabel()
        => _currentFilter switch
        {
            QuickFilter.MyParticipants => "My Participants",
            QuickFilter.OverdueRisk => "Overdue Risk",
            QuickFilter.AssignedLast10Days => "Assigned (Last 10 Days)",
            QuickFilter.AssignedLast30Days => "Assigned (Last 30 Days)",
            QuickFilter.VisitedLast7Days => "Visited (Last 7 Days)",
            _ => "All"
        };
    
    private void OnEnrol() => Navigation.NavigateTo("/pages/candidates/search");

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportParticipants.Command()
            {
                Query = Query!
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
}

/// <summary>
/// UI wrapper for a participant group header: holds the lazily-fetched rows and load state so each
/// group fetches its participants only when expanded.
/// </summary>
public sealed class ParticipantGroupView(ParticipantGroupDto group)
{
    public string Key => group.Key;
    public string Label => group.Label;
    public int Count => group.Count;
    public ParticipantPaginationDto[] Items { get; set; } = [];
    public bool Loaded { get; set; }
    public bool Loading { get; set; }
}