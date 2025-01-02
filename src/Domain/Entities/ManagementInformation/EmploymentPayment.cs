using Cfo.Cats.Domain.Exceptions;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class EmploymentPayment
{
#pragma warning disable CS8618
    internal EmploymentPayment()
    {
    }
#pragma warning restore CS8618

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid ActivityId { get; set; }
    public DateTime ActivityApproved { get; set; }
    public string ParticipantId { get; set; }
    public string ContractId { get; set; }
    public int LocationId { get; set; }
    public string LocationType { get; set; }
    public string TenantId { get; set; }
    public bool EligibleForPayment { get; set; }
    public string? IneligibilityReason { get; set; }

}

public class EmploymentPaymentBuilder
{
    private Guid? _activityId;
    private string? _participantId;
    private string? _contractId;
    private DateTime? _activityApproved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;


    public EmploymentPaymentBuilder WithActivity(Guid activityId)
    {
        _activityId = activityId;
        return this;
    }

    public EmploymentPaymentBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public EmploymentPaymentBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public EmploymentPaymentBuilder WithApproved(DateTime approved)
    {
        _activityApproved = approved;
        return this;
    }

    public EmploymentPaymentBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public EmploymentPaymentBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public EmploymentPaymentBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public EmploymentPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public EmploymentPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public EmploymentPayment Build()
    {
        var payment = new EmploymentPayment()
        {
            ActivityId = _activityId ?? throw new InvalidBuilderException("ActivityId"),
            ActivityApproved = _activityApproved ?? throw new InvalidBuilderException("ActivityApproved"),
            ContractId = _contractId ?? throw new InvalidBuilderException("ContractId"),
            EligibleForPayment = _eligibleForPayment ?? throw new InvalidBuilderException("EligibleForPayment"),
            IneligibilityReason = _eligibleForPayment == false && _ineligibilityReason == null
                                        ? throw new InvalidBuilderException("IneligibilityReason")
                    : _ineligibilityReason,
            LocationId = _locationId ?? throw new InvalidBuilderException("LocationType"),
            LocationType = _locationType ?? throw new InvalidBuilderException("LocationType"),
            ParticipantId = _participantId ?? throw new InvalidBuilderException("ParticipantId"),
            TenantId = _tenantId ?? throw new InvalidBuilderException("TenantId"),
        };
        return payment;
    }
}