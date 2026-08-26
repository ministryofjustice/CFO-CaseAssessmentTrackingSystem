namespace Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Commands.UpdateContractTarget;

public class UpdateContractTargetCommandValidator : AbstractValidator<UpdateContractTargetCommand>
{
    public UpdateContractTargetCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Prison).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Community).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Wings).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Hubs).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PreReleaseSupport).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ThroughTheGate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SupportWork).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HumanCitizenship).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CommunityAndSocial).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Interventions).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Employment).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TrainingAndEducation).GreaterThanOrEqualTo(0);
    }
}
