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
    private Guid? _activityId;
    private string? _activityCategory;
    private string? _activityType;
    private string? _participantId;
    private string? _contractId;
    private DateTime? _activityApproved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;


    public ActivityPaymentBuilder WithActivity(Guid activityId)
    {
        _activityId = activityId;
        return this;
    }

    public ActivityPaymentBuilder WithActivityCategory(string activityCategory)
    {
        _activityCategory = activityCategory;
        return this;
    }

    public ActivityPaymentBuilder WithActivityType(string activityType)
    {
        _activityType = activityType;
        return this;
    }

    public ActivityPaymentBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public ActivityPaymentBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public ActivityPaymentBuilder WithApproved(DateTime approved)
    {
        _activityApproved = approved;
        return this;
    }

    public ActivityPaymentBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public ActivityPaymentBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public ActivityPaymentBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public ActivityPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public ActivityPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public ActivityPayment Build()
    {
        var payment = new ActivityPayment()
        {
            ActivityId = _activityId ?? throw new ApplicationException("ActivityId must be set before calling build"),
            ActivityApproved = _activityApproved ?? throw new ApplicationException("ActivityApproved must be set before calling build"),
            ActivityCategory = _activityCategory ?? throw new ApplicationException("ActivityCategory must be set before calling build"),
            ActivityType = _activityType ?? throw new ApplicationException("ActivityType must be set before calling build"),
            ContractId = _contractId ?? throw new ApplicationException("ContractId must be set before calling build"),
            EligibleForPayment = _eligibleForPayment ?? throw new ApplicationException("EligibleForPayment must be set before calling build"),
            IneligibilityReason = _eligibleForPayment == false && _ineligibilityReason == null 
                                        ? throw new ApplicationException("IneligibilityReason must be set before calling build") 
                    : _ineligibilityReason,
            LocationId = _locationId ?? throw new ApplicationException("LocationId must be set before calling build"),
            LocationType = _locationType ?? throw new ApplicationException("LocationType must be set before calling build"),
            ParticipantId = _participantId ?? throw new ApplicationException("ParticipantId must be set before calling build"),
            TenantId = _tenantId ?? throw new ApplicationException("TenantId must be set before calling build"),
        };
        return payment;
    }
}