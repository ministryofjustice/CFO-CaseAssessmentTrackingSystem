using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyParticipantsLatestEngagementComponent
{
    private bool _loading;

    private Result<IEnumerable<ParticipantEngagementDto>>? Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _loading = true;

            Model = await GetNewMediator().Send(new GetParticipantsLatestEngagement.Query()
            {
                JustMyCases = true
            });
        }
        finally
        {
            _loading = false;
        }
    }

}
