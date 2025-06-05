using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants;

public partial class MovedParticipants
{
    private IEnumerable<GetParticipantsWithAccessEndingSoon.ParticipantWithAccessEndingSoonDto> participantAccess = [];
    protected override async Task OnInitializedAsync()
    {
        participantAccess = await GetNewMediator().Send(new GetParticipantsWithAccessEndingSoon.Query());
        await base.OnInitializedAsync();
    }
    private void View(string participantId)
    {
        Navigation.NavigateTo($"/pages/participants/{participantId}");
    }
}
