namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class InductionPayment
{
#pragma warning disable CS8618 
    internal InductionPayment()
    {
        // required for EF Core
    }
#pragma warning restore CS8618    

    public Guid Id { get;  set; } = Guid.CreateVersion7();

    public DateTime CreatedOn { get;  set; } = DateTime.UtcNow;

    public string ParticipantId { get;  set ; }

    public string SupportWorker { get;  set; }

    public string ContractId { get;  set; }

    public DateTime Induction { get;set; }

    public DateTime? Approved { get; set; }

    public int LocationId { get; set; }

    public string LocationType { get; set; }

    public string TenantId { get; set; }
    
    public bool EligibleForPayment { get; set; }

    public string? IneligibilityReason { get;  set; } 

}

public class InductionPaymentBuilder
{
    private string? _participantId;
    private string? _supportWorker;
    private string? _contractId;
    private DateTime? _induction;
    private DateTime? _approved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;

    public InductionPaymentBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public InductionPaymentBuilder WithSupportWorker(string supportWorker)
    {
        _supportWorker = supportWorker;
        return this;
    }

    public InductionPaymentBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public InductionPaymentBuilder WithInduction(DateTime induction)
    {
        _induction = induction.Date;
        return this;
    }
    public InductionPaymentBuilder WithApproved(DateTime? approved)
    {
        _approved = approved.HasValue ? approved.Value.Date : null;
        return this;
    }

    public InductionPaymentBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public InductionPaymentBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public InductionPaymentBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public InductionPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public InductionPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public InductionPayment Build()
    {

        if(_eligibleForPayment == true && _approved is null)
        {
            throw new ApplicationException("Inductions eligible for payment must have an Approved Date set");
        }

        return new InductionPayment()
        {
            ParticipantId = _participantId ?? throw new ApplicationException("ParticipantId must be set before calling build"), 
            SupportWorker = _supportWorker ?? throw new ApplicationException("SupportWorker must be set before calling build"), 
            ContractId = _contractId ?? throw new ApplicationException("ContractId must be set before calling build"), 
            Approved = _approved,
            Induction = _induction ?? throw new ApplicationException("Induction must be set before calling build"),
            LocationId = _locationId ?? throw new ApplicationException("LocationId must be set before calling build"), 
            LocationType = _locationType ?? throw new ApplicationException("LocationType must be set before calling build"),
            TenantId = _tenantId ?? throw new ApplicationException("TenantId must be set before calling build"),
            EligibleForPayment = _eligibleForPayment ?? throw new ApplicationException("EligibleForPayment must be set before calling build"),
            IneligibilityReason = (_eligibleForPayment == false && _ineligibilityReason == null ) 
                ? throw new ApplicationException("EligibleForPayment must be set before calling build when not eligible for payment") 
                : _ineligibilityReason
        };
    }

}