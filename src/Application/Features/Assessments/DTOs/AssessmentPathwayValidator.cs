using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentPathwayValidator : AbstractValidator<PathwayBase>
{
    public AssessmentPathwayValidator()
    {
        RuleForEach(model => model.Questions)
            .Must(q => q.IsValid())
            .WithMessage("You must select a valid option!");
    }
}