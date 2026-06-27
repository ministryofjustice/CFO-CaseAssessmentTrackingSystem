using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Pages;

public partial class MovedParticipants
{
    private IEnumerable<GetParticipantsWithAccessEndingSoon.ParticipantWithAccessEndingSoonDto> participantAccess = [];

    private IReadOnlyList<BreadcrumbItem> _breadCrumbs = [
        new BreadcrumbItem(ParticipantLinks.Home.Title, ParticipantLinks.Home.Url, false),
        new BreadcrumbItem(ParticipantLinks.MovedParticipants.Title, ParticipantLinks.MovedParticipants.Url, true)
    ];

    protected override async Task OnInitializedAsync()
    {
        participantAccess = await GetNewMediator().Send(new GetParticipantsWithAccessEndingSoon.Query());
        await base.OnInitializedAsync();
    }
    private void View(string participantId) => Navigation.NavigateTo($"/pages/workspace/participants/{participantId}");
}
