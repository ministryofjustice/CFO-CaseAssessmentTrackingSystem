using Cfo.Cats.Application.Features.PerformanceManagement.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Analytics.Components;

public partial class OutcomeQualityDipSampleTimelineComponent
{
    [Parameter, EditorRequired]
    public ParticipantDipSampleDto Participant { get; set; } = null!;
    private bool _hideDetails = false;
}
