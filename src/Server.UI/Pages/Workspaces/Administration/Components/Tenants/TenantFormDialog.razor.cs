using Cfo.Cats.Application.Features.Tenants.Commands.AddEdit;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Tenants;

public partial class TenantFormDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [EditorRequired] [Parameter] public AddEditTenantCommand Model { get; set; } = null!;
    private MudForm? _form;
    private bool _saving;

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

    private void Cancel() => MudDialog.Cancel();
}
