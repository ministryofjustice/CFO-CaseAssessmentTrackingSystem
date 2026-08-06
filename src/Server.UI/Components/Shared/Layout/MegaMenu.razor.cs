using Cfo.Cats.Application.Features.Identity.Commands;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class MegaMenu
{
    [Inject] private IAsyncMenuService MenuService { get; set; } = default!;
    [Inject] private IWorkspacePreferenceService WorkspacePreferenceService { get; set; } = null!;
    [Inject] private ILogger<MegaMenu> Logger { get; set; } = null!;
    [Inject] private IApplicationSettings Settings { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private NavigationMenuModel _menuModel = null!;
    private string? _defaultWorkspace;
    private string? _currentUserId;
    
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await LoadMenuIfNeededAsync();
        await RefreshWorkspacePreferenceAsync();
    }

    private async Task LoadMenuIfNeededAsync()
    {
        if (!_loaded)
        {
            var state = await AuthState;
            _menuModel = await MenuService.GetFeaturesAsync(state.User);
            if (state.User.Identity?.IsAuthenticated == true)
            {
                _currentUserId = state.User.GetUserProfileFromClaim().UserId;
            }
            _loaded = true;
        }
    }

    private async Task RefreshWorkspacePreferenceAsync()
    {
        if (string.IsNullOrWhiteSpace(_currentUserId))
        {
            return;
        }

        try
        {
            _defaultWorkspace = await WorkspacePreferenceService.GetHomePageAsync(_currentUserId);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh workspace preference");
        }
    }
    
    private async Task ToggleWorkspaceBookmark(string workspaceUrl)
    {
        var newDefaultWorkspace = _defaultWorkspace == workspaceUrl ? null : workspaceUrl;
        
        var result = await GetNewMediator().Send(new SetHomePage.Command
        {
            HomePage = newDefaultWorkspace
        });

        if (result.Succeeded)
        {
            _defaultWorkspace = newDefaultWorkspace;
            var message = newDefaultWorkspace != null 
                ? "Default workspace set" 
                : "Default workspace cleared";
            Snackbar.Add(message, Severity.Success);
            
            // Notify other components that the workspace preference changed
            WorkspacePreferenceService.NotifyWorkspacePreferenceChanged();
            
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }

}
