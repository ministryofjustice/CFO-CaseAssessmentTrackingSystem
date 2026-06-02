using System.Data.Entity.Core.Metadata.Edm;
using Cfo.Cats.Application.Common.Interfaces.Identity;
using Cfo.Cats.Application.Common.Interfaces.MultiTenant;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.Commands;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Queries;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments;

public partial class PqaList
{
    [CascadingParameter] private UserProfile UserProfile { get; set; } = null!;
    
    [Inject]
    public IUserService UserService { get; set; } = null!;

    [Inject]
    public ITenantService TenantService { get; set; } = null!;

    [Inject]
    public PQASessionStorage SessionStorage { get; set; } = null!;

    private IDictionary<string, string> _users = null!;

    private IDictionary<string, string> _tenants = null!;

    private int _totalPages = 0;
    private int _totalItems = 0;

    private bool _loading = false;
    private bool _downloading;
    
    private EnrolmentQueueEntryDto[] _data = [];

    private PqaQueueWithPagination.Query Query { get; set; } = new();
    private EnrolmentQueueEntryDto _currentDto = new();
    
    protected override async Task OnInitializedAsync()
    {
        _users = UserService.DataSource
            .Where(d => d.TenantId!.StartsWith(UserProfile.TenantId!))
            .ToDictionary(a => a.Id, e => e.DisplayName);

        _tenants = TenantService.GetVisibleTenants(UserProfile.TenantId!)
                    .ToDictionary(k => k.Id, k => k.Name);

        var cached = await SessionStorage.GetAsync();

        if(cached is { Succeeded: true, Data: { } sd })
        {
            Query.Keyword = sd.Keyword;
            Query.OrderBy = sd.OrderBy;
            Query.PageNumber = sd.PageNumber;
            Query.PageSize = 50;
            Query.SortDirection = sd.SortDirection;
            Query.SupportWorkerId = sd.SupportWorkerId;
            Query.TenantId = sd.TenantId;
        }

        await OnRefresh();
    }

    private void OnRowClick(TableRowClickEventArgs<EnrolmentQueueEntryDto> args)
    {
        if(args?.Item is not null)
        {
            Navigation.NavigateTo($"/pages/qa/enrolments/pqa/{args.Item.Id}");
        }
    }

    private Task PageChanged(int page)
    {
        Query.PageNumber = page;
        return OnRefresh();
    }

    private async Task ClearSearch()
    {
        ResetQuery();
        await OnRefresh();
    }

    private void ResetQuery()
    {
        Query.SupportWorkerId = null;
        Query.TenantId = null;
        Query.OrderBy = "ParticipantId";
        Query.SortDirection = SortDirection.Ascending.ToString();
        Query.PageNumber = 1;
        Query.Keyword = null;
    }

    private Task OnSearch(string text)
    {
        Query.Keyword = text;
        return OnRefresh();
    }

    private Task OnRefreshClicked()
        => OnRefresh();

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;
        var results = await Service.Send(Query);
        if(results is { Succeeded: true, Data: not null})
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
        await SessionStorage.SetAsync(PQASessionData.FromQuery(Query));
    }

    private async Task OnExport()
    {
        try
        {
            _downloading = true;
            var result = await Service.Send(new ExportPqaEnrolments.Command()
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

    private async Task ShowTenantDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", UserProfile! }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Select Tenant", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            Query.TenantId = tenant.TenantId;
            await OnRefresh();
        }
    }
    
    private async Task ShowSupportWorkerDialog()
    {
        var parameters = new DialogParameters<SelectUserDialog>
        {
            { "CurrentUser", UserProfile! }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectUserDialog>("Select Support Worker", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedUser user })
        {
            Query.SupportWorkerId = user.UserId;
            await OnRefresh();
        }
    }
}