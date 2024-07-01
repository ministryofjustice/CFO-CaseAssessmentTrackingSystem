namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentMultipleChoiceQuestionModelValidator : AbstractValidator<AssessmentMultipleChoiceQuestionDto>
{
    public AssessmentMultipleChoiceQuestionModelValidator()
    {
        RuleFor(question => question.Toggles)
            .Must(toggles => toggles is not null && toggles.Count() > 0)
            .WithMessage("You must select at least one option!");
    }
}