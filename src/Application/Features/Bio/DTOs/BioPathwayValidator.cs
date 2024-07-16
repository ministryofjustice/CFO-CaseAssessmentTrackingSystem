namespace Cfo.Cats.Application.Features.Bio.DTOs;

public class BioPathwayValidator : AbstractValidator<PathwayBase>
{
    public BioPathwayValidator()
    {
        RuleForEach(model => model.Questions())
            .Must(q => q.IsValid())
            .WithMessage("You must select a valid option!")
            .OverridePropertyName("Questions");
    }
}