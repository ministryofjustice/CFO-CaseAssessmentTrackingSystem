using Cfo.Cats.Application.Features.Delius.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Transfers.Components;

public partial class OffenderManagerSummaryDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter][EditorRequired] public string ParticipantId { get; set; } = string.Empty;
    public string? Crn { get; set; } = default!;
    protected override async Task OnInitializedAsync()
    {
            var mediator = GetNewMediator();

            ParticipantIdentifierDto[] identifiers = await mediator.Send(new GetParticipantIdentifiers.Query()
            {
                ParticipantId = ParticipantId
            });

            Crn = identifiers.FirstOrDefault(i => i.Type == ExternalIdentifierType.Crn)?.Value;
    }
    private void Close() => MudDialog.Cancel();
}