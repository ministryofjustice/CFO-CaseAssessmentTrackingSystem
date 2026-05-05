using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.InnovationFunds.Commands.EditInnovationFund;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Components.InnovationFunds;

public partial class EditInnovationFundDialog
{
    private MudForm? _form;
    private bool _saving = false;

    [Inject] private IContractService ContractService { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired] public UserProfile CurrentUser { get; set; } = null!;
    [Parameter, EditorRequired] public EditInnovationFundCommand Model { get; set; } = null!;

    protected override void OnParametersSet() =>
        Model.Contract = ContractService.DataSource
            .FirstOrDefault(c => c.Id == Model.Contract?.Id);

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
