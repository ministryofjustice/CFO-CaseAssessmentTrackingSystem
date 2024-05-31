namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentMultipleChoiceQuestionModelValidator : AbstractValidator<AssessmentMultipleChoiceQuestionDto>
{
    public AssessmentMultipleChoiceQuestionModelValidator()
    {
        RuleFor(question => question.Toggles)
            .Must(toggles => toggles.Any(toggle => toggle == true))
            .WithMessage("You must select at least one option!");
    }
}