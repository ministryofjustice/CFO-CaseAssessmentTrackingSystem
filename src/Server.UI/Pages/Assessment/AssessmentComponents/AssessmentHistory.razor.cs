using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Assessment.AssessmentComponents;

public partial class AssessmentHistory
{
    [Parameter, EditorRequired]
    public IEnumerable<ParticipantAssessmentDto> ParticipantAssessments { get; set; } = Enumerable.Empty<ParticipantAssessmentDto>();
    [Parameter, EditorRequired]
    public DateOnly? ConsentDate { get; set; }
}