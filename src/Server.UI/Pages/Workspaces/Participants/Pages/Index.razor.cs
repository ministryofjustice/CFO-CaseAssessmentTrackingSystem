using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Server.UI.Models.Breadcrumb;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class Index
{

    [Inject]
    public IAuthorizationService AuthorizationService { get; set; } = null!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    private bool _allowTransfers = false;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        _allowTransfers = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Transfers)).Succeeded;

        List<BreadcrumbLinkModel> links = [];

        links.Add(ParticipantLinks.All);
        links.Add(ParticipantLinks.AllActivities);
        links.Add(ParticipantLinks.MovedParticipants);

        if(_allowTransfers)
        {
            links.Add(ParticipantLinks.Transfers);    
        }

        links.Add(ParticipantLinks.AllPris);

        Links = links.ToArray();        

    }

}