using Cfo.Cats.Application.Features.Activities.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Activities;

public partial class AddActivityDialog
{
    private MudForm _form = new();
    private bool _saving;

    [CascadingParameter] public required IMudDialogInstance Dialog { get; set; }

    [Parameter, EditorRequired] public required AddActivity.Command Model { get; set; }

    [Parameter] public string ParticipantId { get; set; } = string.Empty;

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form.ValidateAsync();

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