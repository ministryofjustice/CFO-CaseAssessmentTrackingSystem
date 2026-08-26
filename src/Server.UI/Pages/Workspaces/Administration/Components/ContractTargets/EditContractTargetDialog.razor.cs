using System.ComponentModel;
using Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Commands.UpdateContractTarget;
using Cfo.Cats.Infrastructure.Constants;
using MudBlazor.State;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.ContractTargets;

public partial class EditContractTargetDialog
{
    private MudForm? _form;
    private bool _saving;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public UpdateContractTargetCommand Model { get; set; } = null!;

    [Parameter, EditorRequired] public string ContractName { get; set; } = null!;

    [Parameter, EditorRequired] public string Period { get; set; } = null!;

    private void Cancel() => MudDialog.Cancel();

    private async Task Save()
    {
        try
        {
            _saving = true;
            await _form!.ValidateAsync();

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
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
