using Cfo.Cats.Application.Features.Offloc.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.DMS.Offloc;

public partial class OffLocDetails
{

    [Inject]
    public IOfflocService OfflocService { get; set; } = default!;

    [Parameter, EditorRequired] 
    public string NomisNumber { get; set; } = default!;
    
    private Result<PersonalDetailsDto>? PersonalDetails { get; set; } 

    protected override async Task OnInitializedAsync() 
        => PersonalDetails = await OfflocService.GetPersonalDetailsAsync(NomisNumber);
}