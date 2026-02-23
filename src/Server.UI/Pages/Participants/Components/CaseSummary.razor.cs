using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto? ParticipantSummaryDto { get; set; }
    
    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    //This can be called by child components to trigger a reload of the summary information after an update has been made to the participant so panels get updated.
    public async Task Reload()
    {
        var result = await GetNewMediator().Send(new GetParticipantSummary.Query
        {
            ParticipantId = ParticipantSummaryDto?.Id!,
            CurrentUser = UserProfile   
        });

        if (result.Succeeded)
        {
            ParticipantSummaryDto = result.Data;
            StateHasChanged();
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
        }
    }
}