using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Objectives;

public partial class AddObjectiveDialog
{
    private MudForm? _form;
    private bool _saving;
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

            await _form.Validate();

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