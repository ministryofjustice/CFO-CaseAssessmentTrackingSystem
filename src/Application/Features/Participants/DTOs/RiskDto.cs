using Cfo.Cats.Application.Common.Validators;
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
            CreateMap<Risk, RiskDto>(MemberList.None)
                .ReverseMap();
        }
    }

    public class Validator : AbstractValidator<RiskDto>
    { 
        public Validator()
        {
            RuleFor(x => x.ActivityRecommendations)
                .NotEmpty()
                .WithMessage("You must provide activity recommendations")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Activity Recommendations"));

            RuleFor(x => x.ActivityRestrictions)
                .NotEmpty()
                .WithMessage("You must provide activity restrictions")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Activity Restrictions"));

            RuleFor(x => x.AdditionalInformation)
                .NotEmpty()
                .WithMessage("You must provide additional information")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Additional Information"));

            RuleFor(x => x.LicenseConditions)
                .NotEmpty()
                .WithMessage("You must provide license conditions")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "License Conditions"));

            When(x => x.IsSubjectToSHPO == true, () => {
                RuleFor(x => x.NSDCase)
                    .NotEmpty()
                    .WithMessage("You must provide NSD Case");
                
                RuleFor(x => x.NSDCase)
                    .Matches(ValidationConstants.Notes)
                    .WithMessage(string.Format(ValidationConstants.NotesMessage, "NSD Case"));
            });
            
            RuleFor(x => x.SpecificRisk)
                .NotEmpty()
                .WithMessage("You must provide specific risks")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Specific Risks"));

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
