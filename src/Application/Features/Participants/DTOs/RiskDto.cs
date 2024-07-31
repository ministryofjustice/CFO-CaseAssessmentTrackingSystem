using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.DTOs;

[Description("Risk")]
public class RiskDto
{
    [Description("Activity Recommendations")]
    public string? ActivityRecommendations { get; set; }

    [Description("Date Latest Activity Recommendations Received")]
    public DateTime? ActivityRecommendationsReceived { get; set; }

    [Description("Activity Restrictions")]
    public string? ActivityRestrictions { get; set; }

    [Description("Date Latest Activity Restrictions Received")]
    public DateTime? ActivityRestrictionsReceived { get; set; }

    [Description("Additional Information")]
    public string? AdditionalInformation { get; set; }

    [Description("License Conditions")]
    public string? LicenseConditions { get; set; }

    [Description("License/Supervision End Date")]
    public DateTime? LicenseEnd { get; set; }

    [Description("PSF Restrictions")]
    public string? PSFRestrictions { get; set; }

    [Description("Date Latest PSF Restrictions Received")]
    public DateTime? PSFRestrictionsReceived { get; set; }

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
    public bool IsRelevantToCustody { get; set; } = false;

    [Description("Community")]
    public bool IsRelevantToCommunity { get; set; } = false;

    [Description("Sexual Harm Prevention Order (SHPO)")]
    public bool? IsSubjectToSHPO { get; set; }

    [Description("NSD case")]
    public bool? NSDCase { get; set; }

    [Description("Specific Risk(s)")]
    public string? SpecificRisk { get; set; }

    public MappaCategory? MappaCategory { get; set; }
    public MappaLevel? MappaLevel { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Risk, RiskDto>(MemberList.None)
                .ReverseMap();
        }
    }

    public record RiskDetails(RiskLevel RiskToChildren, RiskLevel RiskToPublic, RiskLevel RiskToKnownAdult, RiskLevel RiskToStaff, RiskLevel RiskToSelf);

    public class Validator : AbstractValidator<RiskDto>
    { 
        public Validator()
        {
            RuleFor(x => x.ActivityRecommendations)
                .NotEmpty()
                .WithMessage("You must provide activity recommendations")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Activity Recommendations"));

            RuleFor(x => x.ActivityRecommendationsReceived)
                .NotEmpty()
                .WithMessage("You must provide the date activity recommendations were received")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast);

            RuleFor(x => x.ActivityRestrictions)
                .NotEmpty()
                .WithMessage("You must provide activity restrictions")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Activity Restrictions"));

            RuleFor(x => x.ActivityRestrictionsReceived)
                .NotEmpty()
                .WithMessage("You must provide the date activity restrictions were received")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast);

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

            RuleFor(x => x.LicenseEnd)
                .NotEmpty()
                .WithMessage("You must provide the license end date")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInFuture);

            RuleFor(x => x.PSFRestrictions)
                .NotEmpty()
                .WithMessage("You must provide PSF restrictions")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "PSF Restrictions"));

            RuleFor(x => x.PSFRestrictionsReceived)
                .NotEmpty()
                .WithMessage("You must provide the date PSF restrictions were received")
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage(ValidationConstants.DateMustBeInPast);

            RuleFor(x => x.SpecificRisk)
                .NotEmpty()
                .WithMessage("You must provide specific risks")
                .Matches(ValidationConstants.Notes)
                .WithMessage(string.Format(ValidationConstants.NotesMessage, "Specific Risks"));

            RuleFor(x => x.IsRelevantToCommunity)
                .Equal(true)
                .When(x => x.IsRelevantToCustody is false)
                .WithMessage("You must pick one");

            RuleFor(x => x.IsRelevantToCustody)
                .Equal(true)
                .When(x => x.IsRelevantToCommunity is false)
                .WithMessage("You must pick one");

            When(x => x.IsRelevantToCommunity || x.IsRelevantToCustody, () =>
            {
                RuleFor(x => x.RiskToChildren)
                    .NotNull()
                    .When(x => x.IsRelevantToCommunity)
                    .WithMessage("This option is mandatory in community");

                RuleFor(x => x.RiskToPublic)
                    .NotNull()
                    .When(x => x.IsRelevantToCommunity)
                    .WithMessage("This option is mandatory in community");

                RuleFor(x => x.RiskToKnownAdult)
                    .NotNull()
                    .When(x => x.IsRelevantToCommunity)
                    .WithMessage("This option is mandatory in community");

                RuleFor(x => x.RiskToStaff)
                    .NotNull()
                    .WithMessage("This option is always mandatory");

                RuleFor(x => x.RiskToOtherPrisoners)
                    .NotNull()
                    .When(x => x.IsRelevantToCustody)
                    .WithMessage("This option is mandatory in custody");

                RuleFor(x => x.RiskToSelf)
                    .NotNull()
                    .WithMessage("This option is always mandatory");
            });

            RuleFor(x => x.MappaCategory)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.MappaLevel)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.IsSubjectToSHPO)
                .NotNull()
                .WithMessage("You must answer");

            RuleFor(x => x.NSDCase)
                .NotNull()
                .WithMessage("You must answer");
        }
    }

}
