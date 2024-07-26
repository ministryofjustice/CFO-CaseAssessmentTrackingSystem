using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Risk")]
public class RiskDto
{
    [Description("Activity Recommendations")]
    public string? ActivityRecommendations { get; set; }

    [Description("Activity Restrictions")]
    public string? ActivityRestrictions { get; set; }

    [Description("Additional Information")]
    public string? AdditionalInformation { get; set; }

    [Description("License Conditions")]
    public string? LicenseConditions { get; set; }

    [Description("Risk to Children")]
    public RiskLevel? RiskToChildren { get; set; }

    [Description("Risk to Public")]
    public RiskLevel? RiskToPublic { get; set; }

    [Description("Risk to Known Adult")]
    public RiskLevel? RiskToKnownAdult { get; set; }

    [Description("Risk to Staff")]
    public RiskLevel? RiskToStaff { get; set; }

    [Description("Risk to Other Prisoners")]
    public RiskLevel? RiskToOtherPrisoners { get; set; }

    [Description("Risk to Self")]
    public RiskLevel? RiskToSelf { get; set; }

    [Description("Custody")]
    public bool? IsRelevantToCustody { get; set; }

    [Description("Community")]
    public bool? IsRelevantToCommunity { get; set; }

    [Description("Sexual Harm Prevention Order (SHPO)")]
    public bool? IsSubjectToSHPO { get; set; }

    [Description("NSD case")]
    public string? NSDCase { get; set; }

    [Description("Specific risk")]
    public string? SpecificRisk { get; set; }

    public MappaCategory? MappaCategory { get; set; }
    public MappaLevel? MappaLevel { get; set; }

    // public NoteDto[] Notes { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Risk, RiskDto>(MemberList.None);
        }
    }

    public class Validator : AbstractValidator<RiskDto>
    { 
        public Validator()
        {
            RuleFor(x => x.ActivityRecommendations)
                .NotEmpty()
                .WithMessage("You must provide activity recommendations");

            RuleFor(x => x.ActivityRestrictions)
                .NotEmpty()
                .WithMessage("You must provide activity restrictions");

            RuleFor(x => x.AdditionalInformation)
                .NotEmpty()
                .WithMessage("You must provide additional information");

            RuleFor(x => x.LicenseConditions)
                .NotEmpty()
                .WithMessage("You must provide license conditions");

            RuleFor(x => x.NSDCase)
                .NotEmpty()
                .WithMessage("You must provide NSD Case")
                .When(x => x.IsSubjectToSHPO is true);

            RuleFor(x => x.SpecificRisk)
                .NotEmpty()
                .WithMessage("You must provide specific risks");

            RuleFor(x => x.RiskToChildren)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.RiskToPublic)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.RiskToKnownAdult)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.RiskToStaff)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.RiskToOtherPrisoners)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.RiskToSelf)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.MappaCategory)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.MappaLevel)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.IsRelevantToCommunity)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.IsRelevantToCustody)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.IsSubjectToSHPO)
                .NotNull()
                .WithMessage("You must answer");
        }
    }

}
