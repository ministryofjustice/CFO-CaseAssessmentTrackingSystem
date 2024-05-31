namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentValidator : AbstractValidator<AssessmentDto>
{
    public AssessmentValidator()
    {
        RuleForEach(model => model.Pathways).SetValidator(new AssessmentPathwayValidator());
    }
}
