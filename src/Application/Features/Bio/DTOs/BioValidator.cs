namespace Cfo.Cats.Application.Features.Bio.DTOs;

public class BioValidator : AbstractValidator<BioSurvey>
{
    public BioValidator()
    {
        RuleForEach(model => model.Pathways).SetValidator(new BioPathwayValidator());
    }
}