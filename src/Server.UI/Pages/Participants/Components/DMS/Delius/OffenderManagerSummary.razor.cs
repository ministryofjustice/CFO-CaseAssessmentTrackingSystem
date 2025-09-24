using Cfo.Cats.Application.Features.Delius.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Delius;
public partial class OffenderManagerSummary
{
    [Inject] public IDeliusService DeliusService { get; set; } = default!;

    [Parameter, EditorRequired]
    public string Crn { get; set; } = null!;
    private bool isLoading = true;
    private Result<OffenderManagerSummaryDto>? OffenderManagerSummaryResult { get; set; }
    protected override async Task OnInitializedAsync()
    {
        OffenderManagerSummaryResult = await DeliusService.GetOffenderManagerSummaryAsync(Crn);
        isLoading = false;
    }
}