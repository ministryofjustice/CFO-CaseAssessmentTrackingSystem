using ApexCharts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Services;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Toolbelt.Blazor.HotKeys2;

namespace Cfo.Cats.Server.UI.Components.Shared.Layout;

public partial class HeaderMenu
{

    private MudAutocomplete<string>? _searchBox;

    [Inject] private HotKeys HotKeys { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILogger<HeaderMenu> Logger { get; set; } = null!;
    [Inject] private INotificationService NotificationService { get; set; } = null!;
    
    [CascadingParameter]
    public UserProfile CurrentUser { get; set; } = default!;

    private HotKeysContext? _hotKeysContext;
    private string _homePageUrl = "/";
    private int _notifications;

    protected override async Task OnInitializedAsync()
    {   
        _notifications = await NotificationService.GetNotificationCount(CurrentUser.UserId);
        _hotKeysContext = HotKeys.CreateContext()
            .Add(ModCode.Ctrl, Code.K, () => _searchBox?.FocusAsync() );
        NotificationService.OnRefreshed += OnNotificationsRefreshed;

        await base.OnInitializedAsync();
    }

    private async void OnNotificationsRefreshed() => await InvokeAsync(StateHasChanged);

    protected void GotoNotifiations() => NavigationManager.NavigateTo(MyAreaLinks.Notifications.Href, false);
}
