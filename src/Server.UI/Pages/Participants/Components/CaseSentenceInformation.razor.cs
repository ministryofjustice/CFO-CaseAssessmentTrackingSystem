using Cfo.Cats.Application.Features.Delius.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class CaseSentenceInformation
{
    [Parameter, EditorRequired]
    public required string ParticipantId { get; set; }

    [Parameter, EditorRequired]
    public bool ParticipantIsActive { get; set; } = default!;

    [Inject] public IDeliusService DeliusService { get; set; } = default!;
    public SentenceDetail? Model { get; private set; }
    private Result<OffenderManagerSummaryDto>? offenderManagerSummaryResult { get; set; }
    public record SentenceDetail(
        ParticipantIdentifierDto[] Identifiers,
        ParticipantSupervisorDto Supervisor,
        OffenderManagerSummaryDto? OffenderManagerSummary);

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var mediator = GetNewMediator();

            ParticipantIdentifierDto[] identifiers = await mediator.Send(new GetParticipantIdentifiers.Query()
            {
                ParticipantId = ParticipantId
            });

            var supervisor = await mediator.Send(new GetParticipantSupervisor.Query()
            {
                ParticipantId = ParticipantId
            });

            if (identifiers.Any())
            {
                var Crn = identifiers.FirstOrDefault(i => i.Type == ExternalIdentifierType.Crn);
                if (string.IsNullOrWhiteSpace(Crn?.Value) == false)
                {
                    offenderManagerSummaryResult = await DeliusService.GetOffenderManagerSummaryAsync(Crn.Value);
                }
            }

            Model = new(
            Identifiers: identifiers,
            Supervisor: supervisor?.Data
                        ?? new ParticipantSupervisorDto(),
            OffenderManagerSummary: offenderManagerSummaryResult?.Data
            );

        }
        finally
        {
            await base.OnInitializedAsync();
        }
    }

    public async Task Edit()
    {
        // Show Dialog
        var parameters = new DialogParameters<EditSupervisorDialog>
        {
            { x => x.Model, new AddOrUpdateSupervisor.Command() { ParticipantId = ParticipantId, Supervisor = Model!.Supervisor } }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = await DialogService.ShowAsync<EditSupervisorDialog>
            ("Edit Supervisor", parameters, options);

        var state = await dialog.Result;

        if (!state!.Canceled)
        {
            // Refresh?
        }
    }
}