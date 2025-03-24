using Ardalis.SmartEnum;
using Cfo.Cats.Domain.Entities.PRIs;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class SupportAndReferralPayment
{
#pragma warning disable CS8618
    private SupportAndReferralPayment() { }
#pragma warning restore CS8618

    public static SupportAndReferralPayment CreateNonPayable(Guid priId,
        string participantId,
        string supportType,
        DateOnly activityInput,
        DateOnly approvedDate,
        string supportWorker,
        string contractId,
        string locationType,
        int locationId,
        string tenantId,
        DateOnly paymentPeriod,
        IneligibilityReason reason)
    {
        return new SupportAndReferralPayment
        {
            Id = Guid.CreateVersion7(),
            PriId = priId,
            SupportType = supportType,
            CreatedOn = DateTime.UtcNow,
            ActivityInput = activityInput.ToDateTime(TimeOnly.MinValue),
            Approved = approvedDate.ToDateTime(TimeOnly.MinValue),
            ParticipantId = participantId,
            SupportWorker = supportWorker,
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            TenantId = tenantId,
            EligibleForPayment = false,
            IneligibilityReason = reason.Value,
            PaymentPeriod = paymentPeriod.ToDateTime(TimeOnly.MinValue)
        };
    }
    
    public static SupportAndReferralPayment CreatePayable (
        Guid priId, 
        string participantId, 
        string supportType, 
        DateOnly activityInput, 
        DateOnly approvedDate,
        string supportWorker,
        string contractId,
        string locationType,
        int locationId,
        string tenantId,
        DateOnly paymentPeriod) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            PriId = priId,
            SupportType = supportType,
            CreatedOn = DateTime.UtcNow,
            ActivityInput = activityInput.ToDateTime(TimeOnly.MinValue),
            Approved = approvedDate.ToDateTime(TimeOnly.MinValue),
            ParticipantId = participantId,
            SupportWorker = supportWorker,
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            TenantId = tenantId,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = paymentPeriod.ToDateTime(TimeOnly.MinValue)
        };

    public required Guid Id { get; set; } 
           
    public required Guid PriId { get; set; }
           
    public required string SupportType { get; set; }
           
    public required DateTime CreatedOn { get; set; }

    /// <summary>
    /// The date the activity was created
    /// </summary>
    public required DateTime ActivityInput { get; set; }
    
    public required DateTime Approved { get; set; }
           
    public required string ParticipantId { get; set; }
           
    public required string SupportWorker { get; set; }
           
    public required string ContractId { get; set; }
           
    public required int LocationId { get; set; }
           
    public required string LocationType { get; set; }
           
    public required string TenantId { get; set; }
           
    public required bool EligibleForPayment { get; set; }
           
    public required string? IneligibilityReason { get; set; }
    public required DateTime PaymentPeriod { get; set; }



        //public static PaymentType PreReleaseSupport = new("Pre-Release Support");
        //public static PaymentType ThroughTheGate = new("Through the Gate");

   

}