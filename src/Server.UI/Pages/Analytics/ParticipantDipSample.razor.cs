namespace Cfo.Cats.Server.UI.Pages.Analytics;

public partial class ParticipantDipSample
{
    [Parameter]
    public required Guid SampleId { get; set; }

    [Parameter]
    public required string ParticipantId { get; set; }
}
