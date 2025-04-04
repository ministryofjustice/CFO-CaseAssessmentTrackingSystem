using Cfo.Cats.Application.Features.Participants.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class PersonalDetailsDialog
{
    MudForm form = new();

    [CascadingParameter] private IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public required AddOrUpdatePersonalDetail.Command Model { get; set; }

    bool saving = false;

    private async Task Submit()
    {
        try
        {
            saving = true;

            await form.Validate();

            if (form.IsValid is false)
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
