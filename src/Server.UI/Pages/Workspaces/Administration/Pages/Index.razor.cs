using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class Index
{
    //[Inject] public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter] public Task<AuthenticationState> AuthState { get; set; } = null!;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        // var authState = await AuthState;
        // _allowTransfers = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Transfers))
        //     .Succeeded;

        List<BreadcrumbLinkModel> links = [];

        links.Add(AdministrationLinks.Jobs);
         links.Add(AdministrationLinks.CacheManagement);
        // links.Add(AdministrationLinks.MovedParticipants);
        //
        // if (_allowTransfers)
        // {
        //     links.Add(AdministrationLinks.Transfers);
        // }
        //
        // links.Add(AdministrationLinks.AllPris);

        Links = links.ToArray();
    }
}
