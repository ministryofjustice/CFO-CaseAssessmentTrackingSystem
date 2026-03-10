using Cfo.Cats.Application.Features.Inductions.Commands;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddHubInductionDialog
{
    private MudForm? form;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public LocationDto[]? Locations { get; set; }

    [EditorRequired]
    [Parameter]
    public AddHubInduction.Command? Model { get; set; }

    private bool saving;

    private bool hasParticpantBeenAtThisLocationOnThisDate;

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task Submit()
    {
        try
        {
            saving = true;

            await form!.Validate();

            if (form!.IsValid == false)
            {
                return;
            }

            var result = await GetNewMediator().Send(Model!);
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
            saving = false;
        }
    }

    private async Task CheckParticipantBeenAtThisLocationOnDate()
    {
        if (Model?.InductionDate == null || Model?.Location?.Id == null)
        {
            return;
        }

        hasParticpantBeenAtThisLocationOnThisDate = await GetNewMediator().Send(new GetParticipantWasAtThisLocationCheck.Query()
        {
            ParticipantId = Model!.ParticipantId,
            LocationId = Model.Location!.Id,
            DateAtLocation = Model.InductionDate
        });
    }
}