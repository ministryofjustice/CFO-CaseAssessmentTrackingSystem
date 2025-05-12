using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS;

public partial class ExternalData
{
    private ExternalIdentifierDto[] Identifiers { get; set; } = [];

    [Parameter] [EditorRequired] public string ParticipantId { get; set; } = default!;

    [Parameter]
    [EditorRequired]
    public ConsentStatus ConsentStatus { get; set; } = ConsentStatus.PendingStatus;

    protected override async Task OnInitializedAsync()
    {
        if (ConsentStatus == ConsentStatus.GrantedStatus)
        {
            var mediator = GetNewMediator();

            var query = new GetExternalIdentifiers.Query()
            {
                ParticipantId = ParticipantId
            };

            Identifiers = await mediator.Send(query);
        }
    }
}