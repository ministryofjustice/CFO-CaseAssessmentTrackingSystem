using Cfo.Cats.Application.Features.Inductions.Commands;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Infrastructure.Constants;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class AddWingInductionDialog
{
    private MudForm? form;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [EditorRequired]
    [Parameter]
    public LocationDto[]? Locations { get; set; }

    [EditorRequired]
    [Parameter]
    public AddWingInduction.Command? Model { get; set; }

    private bool saving;

    private bool hasParticpantBeenAtThisLocationOnThisDate;

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        try
        {
            saving = true;

            await form!.ValidateAsync();

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

    private async Task<IEnumerable<LocationDto>> SearchLocations(string value, CancellationToken token)
    {
        if(string.IsNullOrEmpty(value)){
            return new LocationDto[0];
        }
        return await Task.FromResult(Locations!.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }
}