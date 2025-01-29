using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class SupportAndReferralPayment
{
#pragma warning disable CS8618
    internal SupportAndReferralPayment()
    {
    }

#pragma warning restore CS8618

    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid PriId { get; set; }

    public string SupportType { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime Approved { get; set; }

    public string ParticipantId { get; set; }
    
    public string SupportWorker { get; set; }

    public string ContractId { get; set; }

    public int LocationId { get; set; }
    
    public string LocationType { get; set; }

    public string TenantId { get; set; }

    public bool EligibleForPayment { get; set; }

    public string? IneligibilityReason { get; set; }

}

public class SupportAndReferralBuilder
{
    private Guid? _priId;
    private string? _supportType;
    private string? _participantId;
    private string? _contractId;
    private DateTime? _approved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;
    private string? _userId;

    public SupportAndReferralBuilder WithPri(Guid priId)
    {
        _priId = priId;
        return this;
    }

    public SupportAndReferralBuilder WithSupportType(string supportType)
    {
        _supportType = supportType;
        return this;
    }

    public SupportAndReferralBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public SupportAndReferralBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public SupportAndReferralBuilder WithApproved(DateTime approved)
    {
        _approved = approved;
        return this;
    }

    public SupportAndReferralBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public SupportAndReferralBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public SupportAndReferralBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public SupportAndReferralBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public SupportAndReferralBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public SupportAndReferralBuilder WithSupportWorker(string userId)
    {
        _userId = userId;
        return this;
    }

    public SupportAndReferralPayment Build()
    {
        var payment = new SupportAndReferralPayment()
        {
            PriId = _priId ?? throw new ApplicationException("PriId must be set before calling build"),
            Approved = _approved ?? throw new ApplicationException("Approved must be set before calling build"),
            SupportType = _supportType ?? throw new ApplicationException("SupportType must be set before calling build"),
            ContractId = _contractId ?? throw new ApplicationException("ContractId must be set before calling build"),
            EligibleForPayment = _eligibleForPayment ?? throw new ApplicationException("EligibleForPayment must be set before calling build"),
            IneligibilityReason = _eligibleForPayment == false && _ineligibilityReason == null
                                        ? throw new ApplicationException("IneligibilityReason must be set before calling build")
                    : _ineligibilityReason,
            LocationId = _locationId ?? throw new ApplicationException("LocationId must be set before calling build"),
            LocationType = _locationType ?? throw new ApplicationException("LocationType must be set before calling build"),
            ParticipantId = _participantId ?? throw new ApplicationException("ParticipantId must be set before calling build"),
            TenantId = _tenantId ?? throw new ApplicationException("TenantId must be set before calling build"),
            SupportWorker = _userId ?? throw new ApplicationException("Support Worker must be set before calling build")
        };
        return payment;
    }

}