using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class RenameObjectiveDialog
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = default!;

    private MudForm? _form;
    private bool _saving;
    private InitiativeDto? _selectedInitiative;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter, EditorRequired] public required EditObjective.Command Model { get; set; }

    protected override void OnInitialized()
    {
        if (Model.InitiativeId.HasValue)
        {
            _selectedInitiative = InitiativeService.DataSource.FirstOrDefault(i => i.Id == Model.InitiativeId.Value);
        }
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            if (_form is null)
            {
                _saving = false;
                return;
            }

            await _form.ValidateAsync();

            if (_form.IsValid)
            {
                Model.InitiativeId = _selectedInitiative?.Id;
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        finally
        {
            _saving = false;
        }
    }
}