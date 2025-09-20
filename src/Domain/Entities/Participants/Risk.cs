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

    private Risk(Guid id, string participantId, RiskReviewReason reviewReason, int locationId)
    {
        Id = id;
        ParticipantId = participantId;
        ReviewReason = reviewReason;
        LocationId = locationId;
    }

    public static Risk Create(Guid id, string participantId, RiskReviewReason reviewReason, int locationId, string? justification = null) 
    {
        Risk risk = new(id, participantId, reviewReason, locationId)
        {
            ReviewJustification = justification,
            LocationId = locationId
        };
        
        if(reviewReason == RiskReviewReason.NoRiskInformationAvailable)
        {
            risk.AddDomainEvent(new RiskInformationCompletedDomainEvent(risk));
        }
        else 
        {
            risk.AddDomainEvent(new RiskInformationAddedDomainEvent(risk));
        }
        
        return risk;
    }

    public void Complete(string completedBy)
    {
        Completed = DateTime.UtcNow;
        CompletedBy = completedBy;
        AddDomainEvent(new RiskInformationCompletedDomainEvent(this));
    }

    public static Risk Review(Risk from, RiskReviewReason reason, string? justification, int locationId)
    {
        from.Id = Guid.CreateVersion7();
        from.ReviewReason = reason;
        from.ReviewJustification = justification;
        from.LocationId = locationId;
        from.Completed = null;
        from.CompletedBy = null;
        from.RegistrationDetailsJson = null;
        from.Created = null;
        from.CreatedBy = null;
        from.LastModified = null;
        from.LastModifiedBy = null;
        if (reason == RiskReviewReason.NoChange || reason ==RiskReviewReason.NoRiskInformationAvailable)
        {
            from.AddDomainEvent(new RiskInformationReviewedDomainEvent(from));
        }
        return from;
    }

    public string? ActivityRecommendations { get; private set; }
    public DateTime? ActivityRecommendationsReceived { get; private set; }
    public string? ActivityRestrictions { get; private set; }
    public DateTime? ActivityRestrictionsReceived { get; private set; }
    public string? AdditionalInformation { get; private set; }
    public DateTime? Completed { get; private set; }
    public string? CompletedBy { get; private set; }
    public string? LicenseConditions { get; private set; }
    public DateTime? LicenseEnd { get; private set; }
    public bool? NoLicenseEndDate { get; private set; }
    public bool? IsRelevantToCustody { get; private set; }
    public bool? IsRelevantToCommunity { get; private set; }
    public ConfirmationStatus? IsSubjectToSHPO { get; private set; }
    public ConfirmationStatus? NSDCase { get; private set; }
    public string ParticipantId { get; private set; }
    public string? PSFRestrictions { get; private set; }
    public DateTime? PSFRestrictionsReceived { get; private set; }
    public string? ReferrerName { get; private set; }
    public string? ReferrerEmail { get; private set; }
    public DateTime? ReferredOn { get; private set; }
    public string? RegistrationDetailsJson { get; private set; }
    public RiskReviewReason ReviewReason { get; private set; }
    public string? ReviewJustification { get; private set; }
    public RiskLevel? RiskToChildrenInCustody { get; private set; }
    public RiskLevel? RiskToPublicInCustody { get; private set; }
    public RiskLevel? RiskToKnownAdultInCustody { get; private set; }
    public RiskLevel? RiskToStaffInCustody { get; private set; }
    public RiskLevel? RiskToOtherPrisonersInCustody { get; private set; }
    public RiskLevel? RiskToSelfInCustody { get; private set; }
    public ConfirmationStatus? RiskToSelfInCustodyNew { get; private set; }
    public RiskLevel? RiskToChildrenInCommunity { get; private set; }
    public RiskLevel? RiskToPublicInCommunity { get; private set; }
    public RiskLevel? RiskToKnownAdultInCommunity { get; private set; }
    public RiskLevel? RiskToStaffInCommunity { get; private set; }
    public RiskLevel? RiskToSelfInCommunity { get; private set; }
    public ConfirmationStatus? RiskToSelfInCommunityNew { get; private set; }
    public string? SpecificRisk { get; private set; }
    public virtual Participant? Participant { get; private set; }
    public int LocationId { get; private set; }
}
