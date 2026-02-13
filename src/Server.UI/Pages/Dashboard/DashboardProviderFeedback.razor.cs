using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Components.Identity;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class DashboardProviderFeedback
{
    private MudDateRangePicker _picker = null!;
    private bool _showSelect;
    private bool _visualMode = true;
    public string? SelectedTenantId { get; set; }
    public string? SelectedUserId { get; set; }
    public string? SelectedDisplayName { get; set; }
    private DateRange _dateRange { get; set; } = new DateRange(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    protected override void OnInitialized()
    {
        _showSelect = CurrentUser.AssignedRoles is { Length: > 0 };

        // if the current user has access to select, don't set the selected Tenant.
        SelectedTenantId = CurrentUser.TenantId;
        SelectedDisplayName = CurrentUser.TenantName;
    }
    private async Task DisplayOptionsDialog()
	{
        var parameters = new DialogParameters<SelectTenantDialog>
        {
            { "CurrentUser", CurrentUser }           
        };

        var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, FullWidth = false};
        var dialog = await DialogService.ShowAsync<SelectTenantDialog>("Dashboard Options", parameters, options);
        var result = await dialog.Result;

        if(result is { Canceled: false, Data: SelectedTenant tenant})
        {
            SelectedTenantId = tenant.TenantId;
            SelectedDisplayName = tenant.DisplayName;
        }
    }
}