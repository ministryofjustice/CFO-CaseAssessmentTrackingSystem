using Cfo.Cats.Application.Features.Assessments.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddAssessmentDialog
{
    private MudForm form = new();
    private bool saving;

    [CascadingParameter]
    public required IMudDialogInstance Dialog { get; set; }

    [Parameter, EditorRequired]
    public required BeginAssessment.Command Model { get; set; }

    private async Task Submit()
    {
        try
        {
            saving = true;

            await form.ValidateAsync();

            if (form.IsValid is false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model);

            if (result.Succeeded)
            {
                Dialog.Close(DialogResult.Ok(result.Data));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }

        }
        finally
        {
            saving = false;
        }
    }
}