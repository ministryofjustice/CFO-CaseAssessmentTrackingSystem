using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Infrastructure.Constants;
using Cfo.Cats.Infrastructure.Services.Ordnance;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddressDialog(IAddressLookupService AddressLookupService)
{
    MudForm form = new();

    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public required AddOrUpdateContactDetail.Command Model { get; set; }

    bool saving = false;

    async Task<IEnumerable<ParticipantAddressDto>> Search(string searchText, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchText) is false)
        {
            var response = await AddressLookupService.SearchAsync(searchText, CancellationToken.None);

            if (response.Succeeded && response.Data is not null)
            {
                return response.Data;
            }
            else
            {
                Snackbar.Add(response.ErrorMessage, Severity.Error);
            }
        }

        return [];
    }

    private async Task Submit()
    {
        try
        {
            saving = true;

            await form.Validate();

            if(form.IsValid is false)
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
        finally { saving = false; }
    }

}
