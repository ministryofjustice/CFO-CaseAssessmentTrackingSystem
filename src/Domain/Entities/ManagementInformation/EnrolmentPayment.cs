using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class EnrolmentPayment
{
#pragma warning disable CS8618 
    internal EnrolmentPayment()
#pragma warning restore CS8618
    {
        // this is for EF Core
    }

    public Guid Id { get;  set; } = Guid.CreateVersion7();

    public DateTime CreatedOn { get;  set; } = DateTime.UtcNow;

    public string ParticipantId { get;  set ; }

    public string SupportWorker { get;  set; }

    public string ContractId { get;  set; }

    public DateTime ConsentAdded { get; set; }

    public DateTime ConsentSigned { get; set; }

    public DateTime SubmissionToPqa { get; set; }

    public DateTime SubmissionToAuthority { get; set; }

    public int SubmissionsToAuthority { get; set; }

    public DateTime Approved { get; set; }

    public int LocationId { get; set; }

    public string LocationType { get; set; }

    public string TenantId { get; set; }

    public string ReferralRoute { get; set; }
    
    public bool EligibleForPayment { get; set; }

    public string? IneligibilityReason { get;  set; } 

    

}

public class EnrolmentPaymentBuilder
{
    private string? _participantId;
    private string? _supportWorker;
    private string? _contractId;
    private DateTime? _consentAdded;
    private DateTime? _consentSigned;
    private DateTime? _submissionToPqa;
    private DateTime? _submissionToAuthority;
    private int? _submissionsToAuthority;
    private DateTime? _approved;
    private int? _locationId;
    private string? _locationType;
    private string? _tenantId;
    private string? _referralRoute;
    private bool? _eligibleForPayment;
    private string? _ineligibilityReason;

    public EnrolmentPaymentBuilder WithParticipantId(string participantId)
    {
        _participantId = participantId;
        return this;
    }

    public EnrolmentPaymentBuilder WithSupportWorker(string supportWorker)
    {
        _supportWorker = supportWorker;
        return this;
    }

    public EnrolmentPaymentBuilder WithContractId(string contractId)
    {
        _contractId = contractId;
        return this;
    }

    public EnrolmentPaymentBuilder WithConsentAdded(DateTime consentAdded)
    {
        _consentAdded = consentAdded.Date;
        return this;
    }

    public EnrolmentPaymentBuilder WithConsentSigned(DateTime consentSigned)
    {
        _consentSigned = consentSigned.Date;
        return this;
    }

    public EnrolmentPaymentBuilder WithSubmissionToPqa(DateTime submissionToPqa)
    {
        _submissionToPqa = submissionToPqa.Date;
        return this;
    }

    public EnrolmentPaymentBuilder WithSubmissionToAuthority(DateTime submissionToAuthority)
    {
        _submissionToAuthority = submissionToAuthority.Date;
        return this;
    }

    public EnrolmentPaymentBuilder WithSubmissionsToAuthority(int submissionsToAuthority)
    {
        _submissionsToAuthority = submissionsToAuthority;
        return this;
    }

    public EnrolmentPaymentBuilder WithApproved(DateTime approved)
    {
        _approved = approved.Date; // we are not interested in the time.
        return this;
    }

    public EnrolmentPaymentBuilder WithLocationId(int locationId)
    {
        _locationId = locationId;
        return this;
    }

    public EnrolmentPaymentBuilder WithLocationType(string locationType)
    {
        _locationType = locationType;
        return this;
    }

    public EnrolmentPaymentBuilder WithTenantId(string tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public EnrolmentPaymentBuilder WithReferralRoute(string referralRoute)
    {
        _referralRoute = referralRoute;
        return this;
    }

    public EnrolmentPaymentBuilder WithEligibleForPayment(bool eligibleForPayment)
    {
        _eligibleForPayment = eligibleForPayment;
        return this;
    }

    public EnrolmentPaymentBuilder WithIneligibilityReason(string? ineligibilityReason)
    {
        _ineligibilityReason = ineligibilityReason;
        return this;
    }

    public EnrolmentPayment Build()
    {

        return new EnrolmentPayment
        {
            ParticipantId = _participantId ?? throw new ApplicationException("ParticipantId must be set before calling build"), 
            SupportWorker = _supportWorker ?? throw new ApplicationException("SupportWorker must be set before calling build"), 
            ContractId = _contractId ?? throw new ApplicationException("ContractId must be set before calling build"), 
            ConsentAdded = _consentAdded ?? throw new ApplicationException("ConsentAdded must be set before calling build"),
            ConsentSigned = _consentSigned ?? throw new ApplicationException("ConsentSigned must be set before calling build"),
            SubmissionToPqa = _submissionToPqa ?? throw new ApplicationException("SubmissionToPqa must be set before calling build"),
            SubmissionToAuthority = _submissionToAuthority ?? throw new ApplicationException("SubmissionToAuthority must be set before calling build"),
            SubmissionsToAuthority = _submissionsToAuthority ?? throw new ApplicationException("SubmissionsToAuthority must be set before calling build"),
            Approved = _approved ?? throw new ApplicationException("Approved must be set before calling build"),
            LocationId = _locationId ?? throw new ApplicationException("LocationId must be set before calling build"), 
            LocationType = _locationType ?? throw new ApplicationException("LocationType must be set before calling build"),
            TenantId = _tenantId ?? throw new ApplicationException("TenantId must be set before calling build"),
            ReferralRoute = _referralRoute ?? throw new ApplicationException("ReferralRoute must be set before calling build"),
            EligibleForPayment = _eligibleForPayment ?? throw new ApplicationException("EligibleForPayment must be set before calling build"),
            IneligibilityReason = (_eligibleForPayment == false && _ineligibilityReason == null ) 
                ? throw new ApplicationException("EligibleForPayment must be set before calling build when not eligible for payment") 
                : _ineligibilityReason
        };
    }

    
}