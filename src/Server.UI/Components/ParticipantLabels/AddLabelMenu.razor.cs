using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.Features.ParticipantLabels.AddParticipantLabel;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Server.UI.Components.ParticipantLabels;

public partial class AddLabelMenu
{
    /// <summary>
    /// The participant to whom we are adding the label to
    /// </summary>
    [Parameter, EditorRequired]
    public ParticipantId ParticipantId { get; set; }

    /// <summary>
    /// The labels the user has access to select
    /// </summary>
    [Parameter, EditorRequired]
    public LabelDto[] VisibleLabels { get; set; }

    /// <summary>
    /// The labels the participant already has associated with them
    /// </summary>
    [Parameter, EditorRequired]
    public Guid[] AlreadySelected { get; set; }

    /// <summary>
    /// Event that fires when a label has been added.
    /// </summary>
    [Parameter, EditorRequired]
    public EventCallback LabelAdded { get; set; }

    private async Task AddLabel(LabelDto label)
    {
        var response = await Service.Send(new AddParticipantLabelCommand(ParticipantId, new LabelId(label.Id)));
        if (!response.Succeeded)
        {
            Snackbar.Add(response.ErrorMessage, Severity.Error);
        }
        else
        {
            await LabelAdded.InvokeAsync();
        }
    }

}