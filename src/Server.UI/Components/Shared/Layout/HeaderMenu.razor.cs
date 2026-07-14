using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Toolbelt.Blazor.HotKeys2;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class HeaderMenu
{
    [Inject] private HotKeys HotKeys { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IWorkspacePreferenceService WorkspacePreferenceService { get; set; } = null!;
    [Inject] private ILogger<HeaderMenu> Logger { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;
    
    [EditorRequired] [Parameter] public EventCallback OpenSearch { get; set; }
    [Parameter] public EventCallback<EventArgs> OnSettingClick { get; set; }
    [Parameter] public bool MenuOpen { get; set; }
    [Parameter] public EventCallback OnMenuClick { get; set; }
    [Parameter] public EventCallback<bool> OnMenuOpenChanged { get; set; }
    [Parameter] public bool SearchOpen { get; set; }
    [Parameter] public EventCallback<bool> OnSearchOpenChanged { get; set; }
    
    private HotKeysContext? _hotKeysContext;
    private string _homePageUrl = "/";

    protected override async Task OnInitializedAsync()
    {
        await LoadUserHomePage();
        
        _hotKeysContext = HotKeys.CreateContext()
            .Add(ModCode.Ctrl, Code.K, () => OpenSearch.InvokeAsync());
        
        // Refresh HomePage whenever navigation occurs
        NavigationManager.LocationChanged += OnLocationChanged;
        
        // Listen for workspace preference changes from other components (e.g., MegaMenu)
        WorkspacePreferenceService.OnWorkspacePreferenceChanged += OnWorkspacePreferenceChanged;
        
        await base.OnInitializedAsync();
    }

    private async Task OnLocationChangedAsync()
    {
        // Refresh the HomePage from database when navigation occurs
        await LoadUserHomePage();
        await InvokeAsync(StateHasChanged);
    }

    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        try
        {
            await OnLocationChangedAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh home page after navigation");
        }
    }

    private async Task OnWorkspacePreferenceChangedAsync()
    {
        // Refresh the HomePage when another component (MegaMenu) changes it
        await LoadUserHomePage();
        await InvokeAsync(StateHasChanged);
    }

    private async void OnWorkspacePreferenceChanged()
    {
        try
        {
            await OnWorkspacePreferenceChangedAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to refresh home page after workspace preference change");
        }
    }

    private async Task LoadUserHomePage()
    {
        var state = await AuthState;
        if (state.User.Identity?.IsAuthenticated == true)
        {
            var userProfile = state.User.GetUserProfileFromClaim();
            var homePage = await WorkspacePreferenceService.GetHomePageAsync(userProfile.UserId);

            // Use user's HomePage if set, otherwise default to "/"
            _homePageUrl = string.IsNullOrWhiteSpace(homePage) ? "/" : homePage;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            WorkspacePreferenceService.OnWorkspacePreferenceChanged -= OnWorkspacePreferenceChanged;
            _hotKeysContext?.DisposeAsync();
        }
        
        base.Dispose(disposing);
    }
}
