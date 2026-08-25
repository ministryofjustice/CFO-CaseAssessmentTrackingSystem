using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Commands.UpdateContractTarget;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class UpdateContractTargetCommand : ICommand<Result>
{
    public required Guid Id { get; set; }

    [Display(Name = "Prison", Description = "Prison enrolment target")]
    public int Prison { get; set; }

    [Display(Name = "Community", Description = "Community enrolment target")]
    public int Community { get; set; }

    [Display(Name = "Wings", Description = "Wing induction target")]
    public int Wings { get; set; }

    [Display(Name = "Hubs", Description = "Hub induction target")]
    public int Hubs { get; set; }

    [Display(Name = "Pre-Release Support", Description = "Pre-release support target")]
    public int PreReleaseSupport { get; set; }

    [Display(Name = "Through the Gate", Description = "Through the gate support target")]
    public int ThroughTheGate { get; set; }

    [Display(Name = "Support Work", Description = "Support work activity target")]
    public int SupportWork { get; set; }

    [Display(Name = "Human Citizenship", Description = "Human citizenship activity target")]
    public int HumanCitizenship { get; set; }

    [Display(Name = "Community and Social", Description = "Community and social activity target")]
    public int CommunityAndSocial { get; set; }

    [Display(Name = "Interventions", Description = "Interventions and services wraparound support target")]
    public int Interventions { get; set; }

    [Display(Name = "Employment", Description = "Employment payment target")]
    public int Employment { get; set; }

    [Display(Name = "Training and Education", Description = "Training and education payment target")]
    public int TrainingAndEducation { get; set; }
}
