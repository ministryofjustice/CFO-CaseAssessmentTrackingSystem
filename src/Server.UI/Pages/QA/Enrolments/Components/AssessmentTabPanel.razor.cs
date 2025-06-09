using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Participants.DTOs;

namespace Cfo.Cats.Server.UI.Pages.QA.Enrolments.Components
{
    public partial class AssessmentTabPanel
    {

        [Parameter, EditorRequired]
        public ParticipantDto ParticipantDto { get; set; } = default!;

        [Parameter, EditorRequired]
        public ParticipantAssessmentDto? ParticipantAssessmentDto { get; set; } = default!;

    }
}