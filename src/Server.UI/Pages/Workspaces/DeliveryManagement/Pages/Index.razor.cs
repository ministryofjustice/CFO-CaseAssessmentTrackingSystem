using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public partial class Index
{
    [Inject] private IWorkspaceLinkProvider LinkProvider { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = default!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        Links = (await LinkProvider.GetLinksAsync(Workspace.DeliveryManagement, authState.User)).ToArray();
    }
}
