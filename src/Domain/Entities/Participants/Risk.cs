using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Participants;

public class Risk : BaseAuditableEntity<Guid>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Risk()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Risk(Guid id, string participantId)
    {
        Id = id;
        ParticipantId = participantId;
    }

    public static Risk CreateFrom(Guid id, string participantId) => new(id, participantId);

    public string? ActivityRecommendations { get; private set; }
    public string? ActivityRestrictions { get; private set; }
    public string? AdditionalInformation { get; private set; }
    public string? LicenseConditions { get; private set; }
    public bool? IsRelevantToCustody { get; private set; }
    public bool? IsRelevantToCommunity { get; private set; }
    public bool? IsSubjectToSHPO { get; private set; }
    public MappaCategory? MappaCategory { get; private set; }
    public MappaLevel? MappaLevel { get; private set; }
    public string? NSDCase { get; private set; }
    public string ParticipantId { get; private set; }
    public RiskLevel? RiskToChildren { get; private set; }
    public RiskLevel? RiskToPublic { get; private set; }
    public RiskLevel? RiskToKnownAdult { get; private set; }
    public RiskLevel? RiskToStaff { get; private set; }
    public RiskLevel? RiskToOtherPrisoners { get; private set; }
    public RiskLevel? RiskToSelf { get; private set; }
    public string? SpecificRisk { get; set; }

    public Risk AddLicenseConditions(string conditions)
    {
        LicenseConditions = conditions;
        return this;
    }

    public Risk AddSHPO(bool isSubjectToSHPO, string nsdCase)
    {
        IsSubjectToSHPO = IsSubjectToSHPO;
        NSDCase = nsdCase;
        return this;
    }

    public Risk AddSpecificRisk(string specificRisk)
    {
        SpecificRisk = specificRisk;
        return this;
    }

    public Risk AddActivityRecommendations(string recommendations)
    {
        ActivityRecommendations = recommendations;
        return this;
    }

    public Risk AddActivityRestrictions(string restrictions)
    {
        ActivityRestrictions = restrictions;
        return this;
    }

    public Risk AddAdditionalInformation(string information)
    {
        AdditionalInformation = information;
        return this;
    }

    public Risk AddRiskDetails(
        RiskLevel riskToChildren,
        RiskLevel riskToPublic,
        RiskLevel riskToKnownAdult,
        RiskLevel riskToStaff,
        RiskLevel riskToOtherPrisoners,
        RiskLevel riskToSelf,
        bool isRelevantToCustody = false,
        bool isRelevantToCommunity = false)
    {
        RiskToChildren = riskToChildren;
        RiskToPublic = riskToPublic;
        RiskToKnownAdult = riskToKnownAdult;
        RiskToStaff = riskToStaff;
        RiskToOtherPrisoners = riskToOtherPrisoners;
        RiskToSelf = riskToSelf;
        IsRelevantToCustody = isRelevantToCustody;
        IsRelevantToCommunity = isRelevantToCommunity;
        return this;
    }

    public Risk AddMappaDetails(MappaCategory category, MappaLevel level)
    {
        MappaCategory = category;
        MappaLevel = level;
        return this;
    }

}
