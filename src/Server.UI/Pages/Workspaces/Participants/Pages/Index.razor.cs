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
    public Task<AuthenticationState> AuthState { get; set; } = null!;

    private bool _allowTransfers = false;

    private BreadcrumbLinkModel[] Links { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        _allowTransfers = (await AuthorizationService.AuthorizeAsync(authState.User, SecurityPolicies.Transfers)).Succeeded;

        List<BreadcrumbLinkModel> links =
        [
            ParticipantLinks.All,
            ParticipantLinks.AllActivities,
            ParticipantLinks.MovedParticipants,
            ParticipantLinks.AllPris
        ];

        if(_allowTransfers)
        {
            links.Add(ParticipantLinks.Transfers);    
        }

        Links = links.ToArray();        
    }
}