using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components.Summary;

public partial class DetailsSummary
{
    [CascadingParameter]
    public ParticipantSummaryDto ParticipantSummaryDto { get; set; } = default!;
}
