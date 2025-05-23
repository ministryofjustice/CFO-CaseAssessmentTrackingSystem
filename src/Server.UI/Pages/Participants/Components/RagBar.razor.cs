using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class RagBar
{
    [Parameter][EditorRequired]
    public ParticipantAssessmentDto? Model { get; set; } = default!;
}
