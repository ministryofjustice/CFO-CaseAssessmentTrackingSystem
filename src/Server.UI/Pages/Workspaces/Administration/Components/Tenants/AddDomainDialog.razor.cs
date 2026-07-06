using Cfo.Cats.Application.Features.Tenants.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Tenants;

public partial class AddDomainDialog
{
    private MudForm? _form;
    
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [EditorRequired] [Parameter] public AddDomainCommand.Command Model { get; set; } = null!;
    
    private bool _saving;

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form!.ValidateAsync();

            if (!_form!.IsValid)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
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
