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

    private bool _isOffenderManagerSummaryLoading;
    public SentenceDetail? Model { get; private set; }
    public record SentenceDetail(
        ParticipantIdentifierDto[] Identifiers,
        ParticipantSupervisorDto Supervisor,
        OffenderManagerSummaryDto? OffenderManagerSummary);

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var mediator = GetNewMediator();

            var identifiers = await mediator.Send(new GetParticipantIdentifiers.Query()
            {
                ParticipantId = ParticipantId
            });

            var supervisor = await mediator.Send(new GetParticipantSupervisor.Query()
            {
                ParticipantId = ParticipantId
            });

            // Populate Model with internal data first
            Model = new(
                Identifiers: identifiers,
                Supervisor: supervisor?.Data ?? new ParticipantSupervisorDto(),
                OffenderManagerSummary: null
            );

            // Trigger the Delius service call separately
            _ = Task.Run(async () =>
            {
                var offenderManagerSummary = await GetOffenderManagerSummaryAsync(identifiers);
                if (Model != null)
                {
                    Model = Model with { OffenderManagerSummary = offenderManagerSummary };
                    await InvokeAsync(StateHasChanged);
                }
            });
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
    private async Task<OffenderManagerSummaryDto?> GetOffenderManagerSummaryAsync(ParticipantIdentifierDto[] identifiers)
    {
        try
        {
            _isOffenderManagerSummaryLoading = true;
            var crn = identifiers.FirstOrDefault(i => i.Type == ExternalIdentifierType.Crn)?.Value;

            if (!string.IsNullOrWhiteSpace(crn))
            {
                var result = await DeliusService.GetOffenderManagerSummaryAsync(crn);
                if (result.Succeeded)
                {
                    return result.Data;
                }
            }
        }
        finally
        {
            _isOffenderManagerSummaryLoading = false;
        }
        return null;
    }
}