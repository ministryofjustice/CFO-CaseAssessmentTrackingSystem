using Cfo.Cats.Application.Features.PathwayPlans.Commands;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ReviewPathwayPlanDialog
{
    private bool _saving;
    private MudForm? _form;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter, EditorRequired]
    public required ReviewPathwayPlan.Command Model { get; set; }

    private void Cancel() =>
        MudDialog.Cancel();
    
    
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