using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.Features.Labels.Commands.EditLabel;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Components.Labels;

public partial class EditLabelDialog
{
    private MudForm? _form;
    private bool _saving;

    [Inject] private IContractService ContractService { get; set; } = null!;

    private IReadOnlyCollection<ContractDto> AvailableContracts { get; set; } = [];
    private IReadOnlyCollection<ContractDto> SelectedContracts { get; set; } = new HashSet<ContractDto>();

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public UserProfile CurrentUser { get; set; } = null!;

    [Parameter, EditorRequired] public EditLabelCommand Model { get; set; } = null!;

    protected override void OnInitialized()
    {
        AvailableContracts = ContractService.GetVisibleContracts(CurrentUser.TenantId ?? "xxx").ToList();
        SelectedContracts = AvailableContracts
            .Where(c => Model.NewContractIds.Contains(c.Id))
            .ToHashSet();
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Add()
    {
        try
        {
            _saving = true;

            Model.NewContractIds = SelectedContracts.Select(c => c.Id).ToList();

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
                Snackbar.Add(message: result.ErrorMessage, Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }
}
