using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Server.UI.Models;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class CasePathwayPlan
{
    [CascadingParameter(Name = "ParticipantDetails")]
    public ParticipantCascadingDetails? ParticipantDetails { get; set; }
}