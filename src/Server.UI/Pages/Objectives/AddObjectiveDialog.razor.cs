using Cfo.Cats.Application.Common.Interfaces.Initiatives;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class AddObjectiveDialog
{
    [Inject] private IInitiativeService InitiativeService { get; set; } = default!;

    private MudForm? _form;
    private bool _saving;
    private InitiativeDto? _selectedInitiative;

    private InitiativeDto? SelectedInitiative
    {
        get => _selectedInitiative;
        set
        {
            _selectedInitiative = value;
            Model.InitiativeId = value?.Id;
            if (value is null)
            {
                Model.InitiativeStartDate = null;
            }
        }
    }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter, EditorRequired] public required AddObjective.Command Model { get; set; }
    [Parameter, EditorRequired] public required string ParticipantName { get; set; }

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
                MudDialog.Close(DialogResult.Ok(true));
            }
        }
        finally
        {
            _saving = false;
        }
    }
}