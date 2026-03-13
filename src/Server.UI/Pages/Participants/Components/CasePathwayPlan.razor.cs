using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class CasePathwayPlan
{
    [CascadingParameter(Name = "ParticipantDetails")]
    public ParticipantCascadingDetailDto? ParticipantDetails { get; set; }
}