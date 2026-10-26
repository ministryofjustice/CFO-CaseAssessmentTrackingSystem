using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.PathwayPlans.DTOs;
using Cfo.Cats.Application.Features.PRIs.Commands;
using Cfo.Cats.Application.Features.PRIs.DTOs;
using Cfo.Cats.Application.Features.PRIs.Queries;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Pages.PRIs.Components;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Cfo.Cats.Server.UI.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class ActivePRIs
{
    [Inject]
    public CatsSessionStorage SessionStorage { get; set; } = null!;

    [Inject]
    public ILocationService LocationService { get; set; } = null!;

    [Inject]
    public IUserService UserService { get; set; } = null!;
    
    [Inject]
    public IParticipantDialogService ParticipantDialogService { get; set; } = null!;
    
    private UserProfile? UserProfile => CurrentUser;

    [SupplyParameterFromQuery(Name = "ListView")]
    public string? ListView { get; set; }

    private const int DefaultPageSize = 15;
    private bool _downloading;
    private int _totalItems;
    private int _totalPages;
    private PRIPaginationDto[] _data = [];
    private bool Tabular { get; set; } = true;
    private IDictionary<string, string> _users = new Dictionary<string, string>();
    private IDictionary<int, string> _locations = new Dictionary<int, string>();

    private ActivePRIsWithPagination.Query Query { get; set; } = new()
    {
        IncludeOutgoing = true,
        IncludeIncoming = true,
        PageNumber = 1,
        PageSize = DefaultPageSize,
        OrderBy = "Id",
        SortDirection = "Descending"
    };
    private PriTypeFilter _priTypeFilter = PriTypeFilter.All;
    private readonly HashSet<Guid> _expanded = [];
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    [Inject]
    private IServiceScopeFactory ScopeFactory { get; set; } = null!;
    private readonly Dictionary<Guid, ObjectiveTaskDto[]> _tasks = new();

    protected override async Task OnInitializedAsync()
    {
        // Initialise locations and users dictionaries
        _locations = LocationService.GetVisibleLocations(UserProfile!.TenantId!)
            .ToDictionary(k => k.Id, e => e.Name);

        _users = UserService.Summaries
            .Where(d => d.TenantId!.StartsWith(UserProfile.TenantId!))
            .ToDictionary(a => a.Id, e => e.DisplayName);

        Query.CurrentUser = UserProfile;
        
        var cached = await SessionStorage.GetAsync<ActivePRIsSessionData>();
        
        if (cached is { Succeeded: true, Data: { } sd })
        {
            Query.Keyword = sd.Keyword;
            Query.OrderBy = string.IsNullOrWhiteSpace(sd.OrderBy) ? "Id" : sd.OrderBy;
            Query.SortDirection = string.IsNullOrWhiteSpace(sd.SortDirection) ? "Descending" : sd.SortDirection;
            Query.PageNumber = sd.PageNumber;
            Query.IncludeOutgoing = sd.IncludeOutgoing;
            Query.IncludeIncoming = sd.IncludeIncoming;
            Query.CustodySupportWorker = sd.CustodySupportWorker;
            Query.CommunitySupportWorker = sd.CommunitySupportWorker;
            Query.ExpectedReleaseRegionId = sd.ExpectedReleaseRegionId;
            Query.ActiveStatus = sd.ActiveStatus;
            Query.JustMyPris = sd.JustMyPris;
            Query.StatusFilter = sd.StatusFilter;
            Tabular = sd.Tabular;
            _priTypeFilter = sd.PriTypeFilter;
        }

        await OnRefresh();
    }
    
    protected override IQuery<Result<PaginatedData<PRIPaginationDto>>> CreateQuery()
    {
        Query.CurrentUser = UserProfile;
        return Query;
    }

    protected override void OnDataLoaded(PaginatedData<PRIPaginationDto> data)
    {
        base.OnDataLoaded(data);
        _data = data.Items.ToArray();
        _totalPages = data.TotalPages;
        _totalItems = data.TotalItems;
    }

    protected override void OnError(string errorMessage)
    {
        base.OnError(errorMessage);
        _data = [];
        _totalPages = 0;
        _totalItems = 0;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Snackbar.Add(errorMessage, Severity.Error);
        }
    }

    private async Task OnRefresh()
    {
        // The list is loaded through the component's shared mediator (one DbContext),
        // so serialize refreshes to avoid overlapping operations on that context.
        await _refreshLock.WaitAsync();
        try
        {
            // Cached task lists may be stale after a refresh/complete/abandon; collapse and reload on demand
            _tasks.Clear();
            _expanded.Clear();

            await LoadDataAsync();

            // Save session state
            await SessionStorage.SetAsync(ActivePRIsSessionData.FromQuery(Query, Tabular, _priTypeFilter));
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    /// <summary>
    /// Sends a request using a fresh service scope (and so a fresh DbContext), so that it can never
    /// overlap with another operation on the component's shared mediator.
    /// </summary>
    private async Task<T> SendInNewScope<T>(Func<IMediator, Task<T>> send)
    {
        using var scope = ScopeFactory.CreateScope();
        return await send(scope.ServiceProvider.GetRequiredService<IMediator>());
    }

    private async Task TabularChanged(bool? tabular)
    {
        Tabular = tabular ?? true;
        
        // Save the updated tabular state
        await SessionStorage.SetAsync(ActivePRIsSessionData.FromQuery(Query, Tabular, _priTypeFilter));
    }
    
    private async Task OnExport()
    {
        try
        {
            _downloading = true;

            var result = await SendInNewScope(m => m.Send(new ExportActivePRIs.Command
            {
                Request = new ExportActivePRIs.ActivePRIsExportRequest
                {
                    UserId = UserProfile?.UserId,
                    Keyword = Query.Keyword,
                    JustMyPris = Query.JustMyPris,
                    IncludeOutgoing = Query.IncludeOutgoing,
                    IncludeIncoming = Query.IncludeIncoming,
                    CustodySupportWorker = Query.CustodySupportWorker,
                    CommunitySupportWorker = Query.CommunitySupportWorker,
                    ExpectedReleaseRegionId = Query.ExpectedReleaseRegionId,
                    ActiveStatus = Query.ActiveStatus,
                    StatusFilter = Query.StatusFilter,
                    OrderBy = Query.OrderBy,
                    SortDirection = Query.SortDirection
                }
            }));

            if (result.Succeeded)
            {
                Snackbar.Add(ConstantString.ExportSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        catch (Exception)
        {
            Snackbar.Add("An error has occurred while generating the PRIs export.", Severity.Error);
        }
        finally
        {
            _downloading = false;
        }
    }

    private async Task OnSearch(string? text)
    {
        if (Loading)
        {
            return;
        }
        
        Query.Keyword = text ?? string.Empty;
        Query.PageNumber = 1; // Reset to first page on search
        await OnRefresh();
    }

    private async Task SetPriType(PriTypeFilter filter)
    {
        _priTypeFilter = filter;
        
        Query.IncludeOutgoing = filter is PriTypeFilter.All or PriTypeFilter.Outgoing;
        Query.IncludeIncoming = filter is PriTypeFilter.All or PriTypeFilter.Incoming;
        Query.PageNumber = 1; // Reset to first page on filter change
        
        await OnRefresh();
    }

    private static string ParticipantUrl(string participantId) => $"/pages/workspace/participants/{participantId}?from=activepri";

    private void ViewParticipant(PRIPaginationDto pri) => Navigation.NavigateTo(ParticipantUrl(pri.ParticipantId));

    private async Task CreatePriCode()
    {
        var parameters = new DialogParameters<PriGenerateCodeDialog>()
        {
            { x => x.Model, new UpsertPriCode.Command()
            {
                ParticipantId = ""
            } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<PriGenerateCodeDialog>(ConstantString.GeneratePRICode, parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            await OnRefresh();
        }
    }

    private async Task AddActualReleaseDate(PRIPaginationDto pri)
    {
        var parameters = new DialogParameters<AddActualReleaseDateDialog>()
        {
            {
                x => x.Model, new  AddActualReleaseDate.Command()
                {
                    ParticipantId = pri.ParticipantId
                }
            }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<AddActualReleaseDateDialog>(ConstantString.AddActualReleaseDate, parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            await OnRefresh();
        }
    }

    private async Task CompletePri(PRIPaginationDto pri)
    {
        var completePriCommand = new CompletePRI.Command()
        {
            ParticipantId = pri.ParticipantId,
            CompletedBy = CurrentUser.UserId
        };

        Result result;
        try
        {
            result = await SendInNewScope(m => m.Send(completePriCommand));
        }
        catch (Exception)
        {
            Snackbar.Add("An error has occurred while completing the PRI.", Severity.Error);
            return;
        }

        if (result.Succeeded)
        {
            Snackbar.Add($"{ConstantString.PRISuccessfullyCompleted}", Severity.Info);
            await OnRefresh();
        }
        else
        {
            Snackbar.Add($"{result.ErrorMessage}", Severity.Error);
        }
    }

    private async Task AbandonPri(PRIPaginationDto pri)
    {
        var parameters = new DialogParameters<AbandonPriDialog>()
        {
            { x => x.Model, new AbandonPRI.Command()
            {
                ParticipantId = pri.ParticipantId,
                AbandonJustification="",
                AbandonReason=PriAbandonReason.Other,
                AbandonedBy=CurrentUser.UserId!
            } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };

        var dialog = await DialogService.ShowAsync<AbandonPriDialog>(ConstantString.AbandonPRI, parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            await OnRefresh();
        }
    }

    private async Task ShowCustodyWorkerDialog()
    {
        var user = await ParticipantDialogService.PromptForAssigneeAsync(UserProfile!, "Select Custody Support Worker", showAllOption: !string.IsNullOrEmpty(Query.CustodySupportWorker), allOptionLabel: "All Custody Support Workers");
        
        if (user is not null)
        {
            Query.CustodySupportWorker = user.UserId == string.Empty ? null : user.UserId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ShowCommunityWorkerDialog()
    {
        var user = await ParticipantDialogService.PromptForAssigneeAsync(UserProfile!, "Select Community Support Worker", showAllOption: !string.IsNullOrEmpty(Query.CommunitySupportWorker), allOptionLabel: "All Community Support Workers");
        
        if (user is not null)
        {
            Query.CommunitySupportWorker = user.UserId == string.Empty ? null : user.UserId;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task ShowRegionDialog()
    {
        var location = await ParticipantDialogService.PromptForLocationAsync(UserProfile!, l => l.LocationType.IsHub == false && l.LocationType.IsCustody == false, title: "Select Expected Release Region", showAllOption: Query.ExpectedReleaseRegionId.HasValue, allOptionLabel: "All Expected Release Regions");
        
        if (location is not null)
        {
            Query.ExpectedReleaseRegionId = location.Id == 0 ? null : location.Id;
            Query.PageNumber = 1;
            await OnRefresh();
        }
    }

    private async Task OnActiveStatusChanged(bool? activeStatus)
    {
        Query.ActiveStatus = activeStatus;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task OnStatusFilterChanged(PriStatusFilter filter)
    {
        Query.StatusFilter = filter;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task ToggleTasks(PRIPaginationDto pri)
    {
        if (!_expanded.Add(pri.Id))
        {
            _expanded.Remove(pri.Id);
            return;
        }

        if (_tasks.ContainsKey(pri.Id))
        {
            return;
        }

        Result<ObjectiveTaskDto[]> result;
        try
        {
            var query = new GetPriTasks.Query { PriId = pri.Id, CurrentUser = UserProfile! };
            result = await SendInNewScope(m => m.Send(query));
        }
        catch (Exception)
        {
            _expanded.Remove(pri.Id);
            Snackbar.Add("An error has occurred while loading the tasks for this PRI.", Severity.Error);
            return;
        }
        if (result is { Succeeded: true, Data: not null })
        {
            _tasks[pri.Id] = result.Data;
        }
        else
        {
            // Don't cache the failure; collapse the row so the user can retry.
            _expanded.Remove(pri.Id);
            Snackbar.Add(string.IsNullOrEmpty(result.ErrorMessage) ? "Unable to load tasks for this PRI." : result.ErrorMessage, Severity.Error);
        }
    }

    private static Color PriStatusColor(PriStatus status) =>
        status == PriStatus.Completed ? Color.Success
        : status == PriStatus.Abandoned ? Color.Warning
        : Color.Info;

    private async Task OnQuickFilterChanged(bool justMyPris)
    {
        Query.JustMyPris = justMyPris;
        Query.PageNumber = 1;
        await OnRefresh();
    }

    private async Task SortBy(string orderBy)
    {
        if (Query.OrderBy == orderBy)
        {
            Query.SortDirection = Query.SortDirection == "Ascending" ? "Descending" : "Ascending";
        }
        else
        {
            Query.OrderBy = orderBy;
            Query.SortDirection = "Ascending";
        }
        await OnRefresh();
    }

    private async Task ClearSearch()
    {
        Query.Keyword = null;
        Query.CustodySupportWorker = null;
        Query.CommunitySupportWorker = null;
        Query.ExpectedReleaseRegionId = null;
        Query.ActiveStatus = null;
        Query.StatusFilter = PriStatusFilter.Active;
        Query.JustMyPris = false;
        Query.PageNumber = 1;
        Query.OrderBy = "Id";
        Query.SortDirection = "Descending";
        _priTypeFilter = PriTypeFilter.All;
        Query.IncludeOutgoing = true;
        Query.IncludeIncoming = true;
        await OnRefresh();
    }
}
