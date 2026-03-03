using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.Labels.Queries;
using Cfo.Cats.Application.Features.ParticipantLabels.CloseLabel;
using Cfo.Cats.Application.Features.ParticipantLabels.GetParticipantLabels;
using Cfo.Cats.Domain.ParticipantLabels;
using Cfo.Cats.Domain.Participants;
using Cfo.Cats.Server.UI.Components.Dialogs;

namespace Cfo.Cats.Server.UI.Components.ParticipantLabels;

public partial class ParticipantLabelsCard
{

    private bool IncludeClosed { get; set; } = false;
    private LabelDto[] VisibleLabels { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        VisibleLabels = await Service.Send(new GetVisibleLabels.Query(CurrentUser));
    }
    
    protected override GetParticipantLabelsQuery CreateQuery()
        => new(new ParticipantId(ParticipantId));

    private async Task DeleteLabel(ParticipantLabelDto dto)
    {
        if (await ShowConfirmation($"Are you sure you want to remove the {dto.Name} label?", "Confirmation Required"))
        {
            var command = new CloseParticipantLabelCommand(new ParticipantLabelId(dto.ParticipantLabelId));

            var result = await Service.Send(command);
            if (result.Succeeded)
            {
                Snackbar.Add("Label removed");
                await RefreshAsync();
            }
            else
            {
                Snackbar.Add(result.ErrorMessage, Severity.Error);
            }

        }
    }
    private async Task<bool> ShowConfirmation(string message, string title)
    {
        var parameters = new DialogParameters<ConfirmationDialog>
    {
        { x => x.ContentText, message }
    };

        var dialog = await DialogService.ShowAsync<ConfirmationDialog>(title, parameters);
        var result = await dialog.Result;

        return result switch
        {
            { Canceled: false } => true,
            _ => false,
        };
    }
    private async Task LabelAddedEventHandler() => await RefreshAsync();

}