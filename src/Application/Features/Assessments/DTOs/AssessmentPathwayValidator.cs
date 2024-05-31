using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentPathwayValidator : AbstractValidator<AssessmentPathwayDto>
{
    public AssessmentPathwayValidator()
    {
        RuleForEach(model => model.ToggleQuestions)
            .SetValidator(new AssessmentToggleQuestionModelValidator());

        RuleForEach(model => model.CheckboxQuestions)
            .SetValidator(new AssessmentMultipleChoiceQuestionModelValidator());
    }
}