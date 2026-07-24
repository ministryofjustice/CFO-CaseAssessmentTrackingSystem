using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public enum QuickFilter
{
    All,
    MyParticipants,
    OverdueRisk,
    AssignedLast10Days,
    AssignedLast30Days,
    VisitedLast7Days,
    ArchivedLast30Days,
    LicenceEndPeriod
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
    public CatsSessionStorage SessionStorage { get;set; } = null!;

    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [Inject]
    public IParticipantDialogService ParticipantDialogService { get; set; } = null!;
    
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;
    
    [SupplyParameterFromQuery(Name = "keyword")]
    public string? KeywordFromQuery { get; set; }

    private IDictionary<int, string> _locations = null!;
    private IDictionary<string, string> _users = null!;

    private IDictionary<string, string> _tenants = null!;

    private IDictionary<Guid, LabelDto> _labels = null!;
    private int _totalPages;
    private int _totalItems;

    private bool Tabular { get; set; }

    private bool _downloading;
    private bool _canReassign;
    private readonly HashSet<string> _selectedParticipantIds = [];
    private QuickFilter _currentFilter = QuickFilter.All;

    private ParticipantPaginationDto[] _data = [];

    private ParticipantsWithPagination.Query Query { get; } = new()
    {
        JustMyCases = false,
        ListView = ParticipantListView.Default,
        PageNumber = 1,
        PageSize = 10,
        Keyword = null,
        OrderBy = "Id",
        SortDirection = "Ascending"
    };

    private void OnRowClick(TableRowClickEventArgs<ParticipantPaginationDto> args)
    {
        if(args.Item is not null)
        {
            ViewParticipant(args.Item);
        }
    }

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

        var labelsResult = await Service.Send(new GetVisibleLabels.Query(UserProfile));

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
        
        // Check if keyword was provided via query parameter (e.g., from ParticipantById search)
        if (!string.IsNullOrWhiteSpace(KeywordFromQuery))
        {
            Query.Keyword = KeywordFromQuery;
            _currentFilter = QuickFilter.All;
            Query.JustMyCases = false;
            Query.OwnerId = null;
            
            // Clear the query parameter from URL after applying it
            NavigationManager.NavigateTo("/pages/workspace/participants/all", replace: true);
        }
        else
        {
            var cached = await SessionStorage.GetAsync<ParticipantsSessionData>();

            if (cached is { Succeeded: true, Data: { } sd })
            {
                Query.Keyword = sd.Keyword;
                Query.Label = sd.LabelId;
                Query.ListView = sd.ListView;
                Query.Locations = sd.Locations;
                Query.OrderBy = string.IsNullOrWhiteSpace(sd.OrderBy) ? "Id" : sd.OrderBy;
                Query.SortDirection = string.IsNullOrWhiteSpace(sd.SortDirection) ? "Ascending" : sd.SortDirection;
                Query.PageNumber = sd.PageNumber;
                Query.OwnerId = sd.OwnerId;
                Query.TenantId = sd.TenantId;
                Query.RiskDue = sd.RiskDue;
                Query.RecentAction = sd.RecentlyAssigned;
                Tabular = sd.Tabular;
                
                // Sync _currentFilter based on restored state
                _currentFilter = sd.RecentlyAssigned switch
                {
                    RecentParticipantFilter.AssignedLast10Days => QuickFilter.AssignedLast10Days,
                    RecentParticipantFilter.AssignedLast30Days => QuickFilter.AssignedLast30Days,
                    RecentParticipantFilter.VisitedLast7Days => QuickFilter.VisitedLast7Days,
                    RecentParticipantFilter.ArchivedLast30Days => QuickFilter.ArchivedLast30Days,
                    RecentParticipantFilter.LicenceEndPeriod => QuickFilter.LicenceEndPeriod,
                    _ when sd.RiskDue.HasValue => QuickFilter.OverdueRisk,
                    _ when !string.IsNullOrEmpty(sd.OwnerId) => QuickFilter.MyParticipants,
                    _ => QuickFilter.All
                };
            }
        }

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
        ResetQuery();
        Query.RecentAction = filter;
        
        _currentFilter = filter switch
        {
            RecentParticipantFilter.AssignedLast10Days => QuickFilter.AssignedLast10Days,
            RecentParticipantFilter.AssignedLast30Days => QuickFilter.AssignedLast30Days,
            RecentParticipantFilter.ArchivedLast30Days => QuickFilter.ArchivedLast30Days,
            RecentParticipantFilter.LicenceEndPeriod => QuickFilter.LicenceEndPeriod,
            _ => QuickFilter.All
        };

        if (filter == RecentParticipantFilter.ArchivedLast30Days)
        {
            // Archived participants are excluded from the Default list view, so widen it
            // to ensure participants archived in the last 30 days are still shown.
            Query.ListView = ParticipantListView.All;
        }
        
        await OnRefresh();
    }

    private async Task RecentlyVisitedFilterChanged()
    {
        ResetQuery();
        Query.RecentAction = RecentParticipantFilter.VisitedLast7Days;
        _currentFilter = QuickFilter.VisitedLast7Days;
        
        await OnRefresh();
    }

    private async Task OnSearch(string? text)
    {
        Query.Keyword = text;
        await OnRefresh();
    }

    private async Task ListViewChanged(ParticipantListView listView)
    {
        Query.ListView = listView;
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
            
            if (results?.ErrorMessage is not null)
            {
                Snackbar.Add(results.ErrorMessage, Severity.Error);
            }
        }
        await SessionStorage.SetAsync(ParticipantsSessionData.FromQuery(Query, Tabular));
    }

    private void SetParticipantSelection(string participantId, bool isSelected)
    {
        if (isSelected)
        {
            _selectedParticipantIds.Add(participantId);
            return;
        }

        _selectedParticipantIds.Remove(participantId);
    }

    private void HandleParticipantSelectionChanged((string ParticipantId, bool IsSelected) args) => SetParticipantSelection(args.ParticipantId, args.IsSelected);

    private void ClearSelectedParticipants() => _selectedParticipantIds.Clear();

    private async Task ReassignSelectedItems()
    {
        if (_selectedParticipantIds.Count == 0)
        {
            return;
        }

        var wasReassigned = await ParticipantDialogService.PromptForReassignAsync(
            UserProfile, 
            _selectedParticipantIds.ToArray());

        if (wasReassigned)
        {
            ParticipantCacheKey.Refresh();
            _selectedParticipantIds.Clear();
            await OnRefresh();
        }
    }

    private async Task ShowSelectLocationDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(UserProfile);
        
        if (location is not null)
        {
            Query.Locations = [location.Id];
            await OnRefresh();
        }
    }

    private async Task ShowAssigneeDialog()
    {
        var user = await ParticipantDialogService.PromptForAssigneeAsync(UserProfile);
        
        if (user is not null)
        {
            Query.OwnerId = user.UserId;
            await OnRefresh();
        }
    }

    private async Task ShowTenantDialog()
    {
        var tenant = await ParticipantDialogService.PromptForTenantAsync(UserProfile);
        
        if (tenant is not null)
        {
            Query.TenantId = tenant.TenantId;
            await OnRefresh();
        }
    }

    private async Task SortBy(string key)
    {
        if (Query.OrderBy == key)
        {
            Query.SortDirection = Query.SortDirection == nameof(SortDirection.Ascending) 
                ? nameof(SortDirection.Descending) 
                : nameof(SortDirection.Ascending);
        }
        else
        {
            Query.OrderBy = key;
            Query.SortDirection = nameof(SortDirection.Ascending);
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
        Query.OrderBy = "Id"; // Default value - smart defaults will apply in handler
        Query.ListView = ParticipantListView.Default;
        Query.SortDirection = "Ascending"; // Default value
        Query.PageNumber = 1;
        Query.Label = null;
        Query.OwnerId = null;
        Query.RiskDue = null;
        Query.RecentAction = RecentParticipantFilter.All;
        Query.JustMyCases = false;
    }

    private string GetCurrentFilterLabel()
        => _currentFilter switch
        {
            QuickFilter.MyParticipants => "My Participants",
            QuickFilter.OverdueRisk => "Overdue Risk",
            QuickFilter.AssignedLast10Days => "Assigned (Last 10 Days)",
            QuickFilter.AssignedLast30Days => "Assigned (Last 30 Days)",
            QuickFilter.VisitedLast7Days => "Visited (Last 7 Days)",
            QuickFilter.ArchivedLast30Days => "Archived (Last 30 Days)",
            QuickFilter.LicenceEndPeriod => "In Licence End Period",
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

    private void ViewParticipant(ParticipantPaginationDto item)
    {
        var targetUrl = item.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus
            ? $"/pages/enrolments/{item.Id}"
            : $"/pages/workspace/participants/{item.Id}?from=all";
        
        Navigation.NavigateTo(targetUrl);
    }
}
