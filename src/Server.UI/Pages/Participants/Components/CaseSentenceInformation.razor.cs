using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components
{
    public partial class CaseSentenceInformation
    {
        [Parameter, EditorRequired]
        public required string ParticipantId { get; set; }

        [Parameter, EditorRequired]
        public bool ParticipantIsActive { get; set; } = default!;

        public SentenceDetail? Model { get; private set; }

        public record SentenceDetail(
            ParticipantIdentifierDto[] Identifiers,
            ParticipantSupervisorDto Supervisor);

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


                Model = new(
                    Identifiers: identifiers,
                    Supervisor: supervisor?.Data
                        ?? new ParticipantSupervisorDto()
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
}