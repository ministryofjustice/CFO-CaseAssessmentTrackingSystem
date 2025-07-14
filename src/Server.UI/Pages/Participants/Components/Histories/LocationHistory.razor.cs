using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Histories;

public partial class LocationHistory
{
    public IEnumerable<ParticipantLocationHistoryDto> ParticipantLocationHistory { get; set; } = Enumerable.Empty<ParticipantLocationHistoryDto>();

    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; } = string.Empty;

    [CascadingParameter]
    public UserProfile UserProfile { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        await base.OnInitializedAsync();
    }

    async Task Refresh()
    {
        if (!string.IsNullOrEmpty(ParticipantId))
        {
            var query = new GetParticipantLocationHistory.Query()
            {
                ParticipantId = ParticipantId,
                CurrentUser = UserProfile!
            };

            ParticipantLocationHistory = await GetNewMediator().Send(query);       
        }
    }
}