using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Services.Navigation;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Performance.Pages;

public partial class Index
{
    [Inject] private IWorkspaceLinkProvider LinkProvider { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        Links = (await LinkProvider.GetLinksAsync(Workspace.Performance, authState.User)).ToArray();
    }
}
