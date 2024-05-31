namespace Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

public class AssessmentToggleQuestionModelValidator : AbstractValidator<AssessmentToggleQuestionDto>
{
    public AssessmentToggleQuestionModelValidator()
    {
        RuleFor(model => model.SelectedOption)
            .NotEmpty().WithMessage("You must select an option!");
    }
}