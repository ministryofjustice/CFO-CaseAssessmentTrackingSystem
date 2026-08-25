using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.Features.Participants.Commands;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class ReassignParticipantDialog
{
    private MudForm? _form;

    [EditorRequired]
    [Parameter]
    public ReassignParticipants.Command Model { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    
    [Parameter] public UserProfile? UserProfile { get; set; }

    private bool _saving;

    private void Cancel() => MudDialog.Close();

    private async Task Submit()
    {
        try
        {
            _saving = true;

            await _form!.ValidateAsync();

            if (_form.IsValid)
            {
                var result = await GetNewMediator().Send(Model);
                if (result.Succeeded)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                    Snackbar.Add($"{Model.ParticipantIdsToReassign.Length} {(Model.ParticipantIdsToReassign.Length == 1 ? "participant" : "participants")} reassigned successfully", Severity.Success);
                }
                else
                {
                    Snackbar.Add(result.ErrorMessage, Severity.Error);
                }
            }
        }
        finally
        {
            _saving = false;
        }
    }

    private void OnUserSelectedChanged(ApplicationUserDto? dto)
    {
        if (dto != null)
        {
            Model.AssigneeId = dto.Id;
        }
        else
        {
            Model.AssigneeId = null; 
        }
    }
}
