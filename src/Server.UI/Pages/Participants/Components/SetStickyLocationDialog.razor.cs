using Cfo.Cats.Application.Features.Candidates.Commands;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class SetStickyLocationDialog
{

    private SetCandidateStickyLocation.Command? Model { get; set; }

    private MudForm? _form;
    private bool _saving;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired][Parameter] public string ParticipantId { get; set; } = default!;

    protected override void OnInitialized()
    {
        Model = new SetCandidateStickyLocation.Command()
        {
            ParticipantId = ParticipantId
        };
    }

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form!.Validate();

            if (_form!.IsValid == false)
            {
                Snackbar.Add("Failed to set sticky location", Severity.Error);
                return;
            }

            var result = await Service.Send(Model!); 
            if (result.Succeeded)
            {
                MudDialog.Close(DialogResult.Ok(true));
                Snackbar.Add(ConstantString.SaveSuccess, Severity.Info);
            }
            else
            {
                Snackbar.Add("Failed to set sticky location", Severity.Error);
            }
        }
        finally
        {
            _saving = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}