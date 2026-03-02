using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.QA.Activities.Components;

public partial class ActivityQaDetails
{
    [Parameter, EditorRequired] public ActivityQaDetailsDto Activity { get; set; } = null!;

    private bool _hasParticipantBeenAtThisLocationOnThisDate;

    protected override async Task OnInitializedAsync()
    {
        await CheckParticipantBeenAtThisLocationOnDate();
        await base.OnInitializedAsync();
    }

    private async Task CheckParticipantBeenAtThisLocationOnDate() =>
        _hasParticipantBeenAtThisLocationOnThisDate = await GetNewMediator().Send(
            new GetParticipantWasAtThisLocationCheck.Query()
            {
                ParticipantId = Activity.ParticipantId,
                LocationId = Activity.Location!.Id,
                DateAtLocation = Activity.CommencedOn
            });
}