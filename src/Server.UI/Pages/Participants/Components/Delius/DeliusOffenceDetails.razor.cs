using Cfo.Cats.Application.Features.Delius.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Delius;

public partial class DeliusOffenceDetails
{
    private bool _isLoading = true;


    private MainOffenceDto? _selectedOffence;
    private Func<MainOffenceDto?, string> _convertToString = o =>
    {
        return o == null ? string.Empty :   
            $"{o.OffenceDescription} - {o.OffenceDate} {(o.Disposals.All(d => d.TerminationDate is not null) ? " (terminated)" : string.Empty)} ";
    };

    [Inject] public IDeliusService DeliusService { get; set; } = default!;

    [Parameter, EditorRequired] 
    public string Crn { get; set; } = default!;

    private Result<OffenceDto>? OffenceResult { get; set; }

    protected override async Task OnInitializedAsync()
    {
        OffenceResult = await DeliusService.GetOffencesAsync(Crn);
        _selectedOffence = OffenceResult.Data?.MainOffences?.FirstOrDefault();
        _isLoading = false;
    }

    private async Task Display(MainOffenceDto context)
    {
        var options = new DialogOptions()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.ExtraExtraLarge,
            CloseButton = true,
            
        };
        var parameters = new DialogParameters<MainOffenceDialog>()
        {
            {
                x => x.Disposals,
                context.Disposals
            }
        };

        await DialogService.ShowAsync<MainOffenceDialog>("Disposal Details", parameters, options);
    }
}