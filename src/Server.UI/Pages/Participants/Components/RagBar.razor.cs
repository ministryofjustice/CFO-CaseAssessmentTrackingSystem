using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.Queries;

namespace Cfo.Cats.Server.UI.Pages.Participants.Components;

public partial class RagBar
{
    [Parameter, EditorRequired]
    public string ParticipantId { get; set; } = null!;

    [Parameter]
    public Guid? AssessmentId { get; set; }

    protected override IQuery<Result<ParticipantAssessmentDto[]>> CreateQuery() 
        => new GetAssessmentScores.Query()
        {
            ParticipantId = ParticipantId,
            AssessmentId = AssessmentId
        };
        
}
