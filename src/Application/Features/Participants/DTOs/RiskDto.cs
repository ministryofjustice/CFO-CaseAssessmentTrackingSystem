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

}
