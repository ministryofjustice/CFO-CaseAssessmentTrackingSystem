using Cfo.Cats.Application.Features.Assessments.Commands;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddAssessmentDialog
{
    private MudForm _form = new();
    private bool _saving;

    [CascadingParameter]
    public required IMudDialogInstance Dialog { get; set; }

    [Parameter, EditorRequired]
    public required BeginAssessment.Command Model { get; set; }

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form.ValidateAsync();

            if (_form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                Dialog.Close(DialogResult.Ok(result.Data));
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
