using AutoMapper;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.PerformanceManagement.EventHandlers;
using Cfo.Cats.Server.UI.Components.Identity;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class PerformanceDashboard
{
    private MudDateRangePicker _picker = null!;
    private DateRange _dateRange { get; set; } = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    private bool _visualMode = true;
    private bool _downloading = false;
    public string? SelectedTenantId { get; set; }
    public string? SelectedDisplayName { get; set; }

    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;

    protected override void OnInitialized()
    {
        SelectedTenantId = CurrentUser.TenantId;
        SelectedDisplayName = CurrentUser.TenantName;
    }

    private async Task DisplayOptionsDialog()
    {
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", CurrentUser }
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false };
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Dashboard Options", parameters, options);
        var result = await dialog.Result;

        if (result is { Canceled: false, Data: SelectedTenant tenant })
        {
            SelectedTenantId = tenant.TenantId;
            SelectedDisplayName = tenant.DisplayName;
        }
    }

}