using Cfo.Cats.Infrastructure.Configurations;
using Cfo.Cats.Server.UI.Models.NavigationMenu;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class MegaMenu : IDisposable
{
    [Inject] private IAsyncMenuService MenuService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [Inject] private AppConfigurationSettings Settings { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter] public bool Open { get; set; }
    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    private NavigationMenuModel _menuModel = null!;
    
    private bool _loaded;

    protected override void OnInitialized() => NavigationManager.LocationChanged += OnLocationChanged;

    protected override async Task OnParametersSetAsync()
    {
        if (Open && !_loaded)
        {
            await LoadAsync();
        }
    }

    private async Task LoadAsync()
    {
        var state = await AuthState;
        _menuModel = await MenuService.GetFeaturesAsync(state.User);
        _loaded = true;
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

    public void Dispose() => NavigationManager.LocationChanged -= OnLocationChanged;
}
