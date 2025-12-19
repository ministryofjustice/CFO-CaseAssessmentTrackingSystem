namespace Cfo.Cats.Server.UI.Pages.Participants.Components;
public partial class CasePathwayPlan
{
    [Parameter]
    [EditorRequired]
    public string ParticipantId { get; set; } = null!;
    
    [Parameter, EditorRequired]
    public bool ParticipantIsActive { get; set; }
}