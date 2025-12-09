using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.Labels;

public partial class EditLabelDialog
{
    private MudForm? _form;
    private bool _saving = false;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public UserProfile CurrentUser { get; set; } = null!;

    [Parameter, EditorRequired] public EditLabel.Command Model { get; set; } = null!;

    private void Cancel() => MudDialog.Cancel();

    private async Task Add()
    {
        try
        {
            _saving = true;
            await _form!.Validate();

            if (_form!.IsValid == false)
            {
                return;
            }

            var result = await Service.Send(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(message: result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }
}