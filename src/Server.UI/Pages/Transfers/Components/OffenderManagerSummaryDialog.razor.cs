using Cfo.Cats.Application.Features.Delius.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Server.UI.Pages.Transfers.Components
{
    public partial class OffenderManagerSummaryDialog
    {
        private bool isLoading = true;
        private OffenderManagerSummaryDto? offenderManagerSummary{ get; set; }
        private Result<OffenderManagerSummaryDto>? offenderManagerSummaryResult { get; set; }
        
        [Inject] public IDeliusService DeliusService { get; set; } = default!;
        
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

        [Parameter][EditorRequired] public string ParticipantId { get; set; } = string.Empty;
      
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = true;
                offenderManagerSummary = null;
                var mediator = GetNewMediator();

                ParticipantIdentifierDto[] identifiers = await mediator.Send(new GetParticipantIdentifiers.Query()
                {
                    ParticipantId = ParticipantId
                });

                if (identifiers.Any())
                {
                    var Crn = identifiers.FirstOrDefault(i => i.Type == ExternalIdentifierType.Crn);
                    if (string.IsNullOrWhiteSpace(Crn?.Value) == false)
                    {
                        offenderManagerSummaryResult = await DeliusService.GetOffenderManagerSummaryAsync(Crn.Value);
                        if (offenderManagerSummaryResult.Succeeded)
                        {
                            offenderManagerSummary = offenderManagerSummaryResult.Data;
                        }
                    }
                }
            }
            finally
            {
                isLoading = false;
            }
        }
        void Close() => MudDialog.Cancel();
    }
}