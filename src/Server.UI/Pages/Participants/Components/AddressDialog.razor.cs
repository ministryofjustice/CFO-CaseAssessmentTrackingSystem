using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Infrastructure.Services.Ordnance;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddressDialog(IAddressLookupService addressLookupService)
{
    private MudForm _form = new();
    private bool _saving = false;

    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public required AddOrUpdateContactDetail.Command Model { get; set; }
    
    private async Task<IEnumerable<ParticipantAddressDto>> Search(string searchText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText) is false)
        {
            var response = await addressLookupService.SearchAsync(searchText, CancellationToken.None);

            if (response is { Succeeded: true, Data: not null })
            {
                return response.Data;
            }

            Snackbar.Add(response.ErrorMessage, Severity.Error);
        }

        return [];
    }

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form.Validate();

            if (_form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                Dialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }

        }
        finally
        {
            _saving = false;
        }
    }

}
