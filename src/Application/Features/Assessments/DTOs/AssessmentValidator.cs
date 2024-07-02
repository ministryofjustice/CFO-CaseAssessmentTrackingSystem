namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentValidator : AbstractValidator<Cfo.Cats.Application.Features.Assessments.DTOs.Assessment>
{
    public AssessmentValidator()
    {
        RuleForEach(model => model.Pathways).SetValidator(new AssessmentPathwayValidator());
    }
}
