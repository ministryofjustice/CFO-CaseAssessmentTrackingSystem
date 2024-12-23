using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ActivityPayment
{
#pragma warning disable CS8618 
    internal ActivityPayment() { }
#pragma warning restore CS8618

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ActivityId { get; set; }
    public string ActivityCategory { get; set; } = default!;
    public string ActivityType { get; set; } = default!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ActivityApproved { get; set; }
    public string ParticipantId { get; set; }
    public string ContractId { get; set; }
    public int LocationId { get; set; }
    public string LocationType { get; set; }
    public string TenantId { get; set; }
    public bool EligibleForPayment { get; set; }
    public string? IneligibilityReason { get; set; }
}

public class ActivityPaymentBuilder
{
    ActivityPayment activityPayment;

    public ActivityPaymentBuilder()
    {
        activityPayment = new();
    }

    public ActivityPaymentBuilder WithActivity(Guid activityId)
    {
        activityPayment.ActivityId = activityId;
        return this;
    }

    public ActivityPaymentBuilder WithActivityCategory(string activityCategory)
    {
        activityPayment.ActivityCategory = activityCategory;
        return this;
    }

    public ActivityPaymentBuilder WithActivityType(string activityType)
    {
        activityPayment.ActivityCategory = activityType;
        return this;
    }

    public ActivityPaymentBuilder WithParticipantId(string participantId)
    {
        activityPayment.ParticipantId = participantId;
        return this;
    }

    public ActivityPaymentBuilder WithContractId(string contractId)
    {
        activityPayment.ContractId = contractId;
        return this;
    }

    public ActivityPaymentBuilder WithApproved(DateTime approved)
    {
        activityPayment.ActivityApproved = approved;
        return this;
    }

    public ActivityPaymentBuilder WithLocationId(int locationId)
    {
        activityPayment.LocationId = locationId;
        return this;
    }

    public ActivityPaymentBuilder WithLocationType(string locationType)
    {
        activityPayment.LocationType = locationType;
        return this;
    }

    public ActivityPaymentBuilder WithTenantId(string tenantId)
    {
        activityPayment.TenantId = tenantId;
        return this;
    }

    public ActivityPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        activityPayment.EligibleForPayment = eligibleForPayment;
        return this;
    }

    public ActivityPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        activityPayment.IneligibilityReason = ineligibilityReason;
        return this;
    }

    public ActivityPayment Build()
    {
        return activityPayment;
    }
}