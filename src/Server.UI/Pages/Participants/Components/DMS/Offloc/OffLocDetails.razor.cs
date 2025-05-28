using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Offloc;

public partial class OffLocDetails
{

    [Inject]
    public IOfflocService OfflocService { get; set; } = default!;

    [Parameter, EditorRequired] 
    public string NomisNumber { get; set; } = default!;
    
    private Result<SentenceDataDto>? SentenceData { get; set; } 

    protected override async Task OnInitializedAsync() 
        => SentenceData = await OfflocService.GetSentenceDataAsync(NomisNumber);
}