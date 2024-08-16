namespace Cfo.Cats.Application.Features.Bios.DTOs;

public class BioValidator : AbstractValidator<Bio>
{
    public BioValidator()
    {
        RuleForEach(model => model.Pathways).SetValidator(new BioPathwayValidator());
    }
}