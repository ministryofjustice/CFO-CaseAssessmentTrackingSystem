using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Components.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Dashboard;

public partial class DashboardProviderFeedback
{
    private MudDateRangePicker _picker = null!;
     private bool _visualMode = true;

    private string? SelectedTenantId { get; set; }
    private string? SelectedDisplayName { get; set; }
    private DateRange DateRange { get; set; } = new(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = null!;
    
    [CascadingParameter] 
    private Task<AuthenticationState> AuthState { get; set; } = null!;
    
    private bool _isQa2User;
    
    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;

        _isQa2User = (await AuthService.AuthorizeAsync(state.User, SecurityPolicies.SeniorInternal)).Succeeded;
     
        SelectedTenantId = CurrentUser.TenantId;
        SelectedDisplayName = CurrentUser.TenantName;
        
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