using System.ComponentModel;
using ActualLab.Fusion;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.Locations;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;
using Cfo.Cats.Server.UI.Components.Locations;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Pages.Participants;

public partial class ParticipantsV2
{

     [Inject]
    public ILocationService LocationService { get; set; } = null!;

    [Inject]
    public IUserService UserService {get;set;} = null!;

    [Inject]
    public ITenantService TenantService {get;set;} = null!;

    [Inject]
    public ParticipantsSessionStorage SessionStorage { get;set; } = null!;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = null!;

    private IDictionary<int, string> _locations = null!;
    private IDictionary<string, string> _users = null!;

    private IDictionary<string, string> _tenants = null!;

    private IDictionary<Guid, LabelDto> _labels = new Dictionary<Guid, LabelDto>();
    private int _totalPages = 0;

    private bool _downloading = false;

    private ParticipantPaginationDto[] _data = [];

    private ParticipantsWithPagination.Query Query { get; set; } = new ParticipantsWithPagination.Query()
    {
        JustMyCases = false,
        ListView = ParticipantListView.Default,
        PageNumber = 1,
        PageSize = 10,
        Keyword = null

    };

    protected override async Task OnInitializedAsync()
    {
        _locations = LocationService.GetVisibleLocations(UserProfile.TenantId!)
                        .ToDictionary( k => k.Id, e => e.Name );

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
        }

        await OnRefresh();
    }

    private async Task LocationValueChanged(int? locationId)
    {
        Query.Locations = locationId == null ? [] : [locationId.Value];
        await OnRefresh();
    }

    private async Task LabelsValueChanged(LabelDto? labelDto)
    {
        Query.Label = labelDto is not null ? new LabelId(labelDto.Id) : null;
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
        }
        else
        {
            _data = [];
            _totalPages = 0;
        }
        await SessionStorage.SetAsync(ParticipantsSessionData.FromQuery(Query));
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
        Query.OwnerId = UserProfile.UserId;
        await OnRefresh();
    }

    private async Task ApplyOverdueRiskFilter()
    {
        ResetQuery();
        Query.RiskDue = DateTime.UtcNow.Date;
        Query.SortDirection = SortDirection.Ascending.ToString();
        Query.OrderBy = "RiskDue";
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
    }
    
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

    private void ViewParticipant(ParticipantPaginationDto item)
    {
        var targetUrl = item.EnrolmentStatus == EnrolmentStatus.IdentifiedStatus
            ? $"/pages/enrolments/{item.Id}"
            : $"/pages/participants/{item.Id}";
        
        Navigation.NavigateTo(targetUrl);
    }
}