using Cfo.Cats.Domain.Common.Entities;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Events;

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

    public static Risk CreateFrom(Guid id, string participantId) 
    {
        Risk r = new(id, participantId);
        r.AddDomainEvent(new RiskInformationAddedDomainEvent(r));
        return r;
    }

    public string? ActivityRecommendations { get; private set; }
    public DateTime? ActivityRecommendationsReceived { get; private set; }
    public string? ActivityRestrictions { get; private set; }
    public DateTime? ActivityRestrictionsReceived { get; private set; }
    public string? AdditionalInformation { get; private set; }
    public string? LicenseConditions { get; private set; }
    public DateTime? LicenseEnd { get; private set; }
    public bool? IsRelevantToCustody { get; private set; }
    public bool? IsRelevantToCommunity { get; private set; }
    public bool? IsSubjectToSHPO { get; private set; }
    public MappaCategory? MappaCategory { get; private set; }
    public MappaLevel? MappaLevel { get; private set; }
    public bool? NSDCase { get; private set; }
    public string ParticipantId { get; private set; }
    public string? PSFRestrictions { get; set; }
    public DateTime? PSFRestrictionsReceived { get; set; }
    public RiskLevel? RiskToChildren { get; private set; }
    public RiskLevel? RiskToPublic { get; private set; }
    public RiskLevel? RiskToKnownAdult { get; private set; }
    public RiskLevel? RiskToStaff { get; private set; }
    public RiskLevel? RiskToOtherPrisoners { get; private set; }
    public RiskLevel? RiskToSelf { get; private set; }
    public string? SpecificRisk { get; private set; }
}
