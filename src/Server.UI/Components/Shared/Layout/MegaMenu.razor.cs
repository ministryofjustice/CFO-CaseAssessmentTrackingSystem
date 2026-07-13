using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.Commands;
using Cfo.Cats.Domain.Identity;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class MegaMenu
{
    [Inject] private IAsyncMenuService MenuService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IWorkspacePreferenceService WorkspacePreferenceService { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    private NavigationMenuModel _menuModel = null!;
    private string? _defaultWorkspace;
    private UserProfile? _userProfile;
    private bool _previousOpenState;
    
    private bool _loaded;

    protected override void OnInitialized() => NavigationManager.LocationChanged += OnLocationChanged;

    protected override async Task OnParametersSetAsync()
    {
        // Detect when menu is newly opened (transition from closed to open)
        var justOpened = Open && !_previousOpenState;
        _previousOpenState = Open;
        
        if (Open)
        {
            await LoadMenuIfNeededAsync();
            
            // Always refresh workspace preference, especially when menu just opened
            if (justOpened || _userProfile is not null)
            {
                await RefreshWorkspacePreferenceAsync();
            }
        }
    }

    private async Task LoadMenuIfNeededAsync()
    {
        if (!_loaded)
        {
            var state = await AuthState;
            _menuModel = await MenuService.GetFeaturesAsync(state.User);
            _userProfile = state.User.GetUserProfileFromClaim();
            _loaded = true;
        }
    }

    private async Task RefreshWorkspacePreferenceAsync()
    {
        if (_userProfile is not null)
        {
            var userManager = ScopedServices.GetRequiredService<UserManager<ApplicationUser>>();
            // Get fresh data from database
            var user = await userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _userProfile.UserId);
            
            _defaultWorkspace = user?.HomePage;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task CloseAsync()
    {
        Open = false;
        await OpenChanged.InvokeAsync(false);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (Open)
        {
            Open = false;
            _ = OpenChanged.InvokeAsync(false);
            InvokeAsync(StateHasChanged);
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
        base.Dispose(disposing);
    }
}
