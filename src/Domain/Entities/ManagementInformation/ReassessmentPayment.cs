using Cfo.Cats.Domain.Entities.Assessments;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ReassessmentPayment
{
#pragma warning disable CS8618
    private ReassessmentPayment() { }
#pragma warning restore CS8618

    public static ReassessmentPayment CreatePayable(
        Guid assessmentId,
        DateTime assessmentCompleted,
        DateTime assessmentCreated,
        string participantId,
        string tenantId,
        string supportWorker, 
        string contractId,
        int locationId,
        string locationType)
    {
        var dates = new[]
        {
            assessmentCompleted.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };

        return new()
        {
            Id = Guid.CreateVersion7(),
            AssessmentId = assessmentId,
            AssessmentCompleted = assessmentCompleted,
            AssessmentCreated = assessmentCreated,
            ParticipantId = participantId,
            TenantId = tenantId,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max(),
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            SupportWorker = supportWorker!
        };
    }

    public static ReassessmentPayment CreateNonPayable(
        Guid assessmentId,
        DateTime assessmentCompleted,
        DateTime assessmentCreated,
        string participantId,
        string tenantId,
        string supportWorker,
        string contractId,
        int locationId,
        string locationType,
        IneligibilityReason ineligibilityReason)
    {
        var dates = new[]
        {
            assessmentCompleted.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        };

        return new()
        {
            Id = Guid.CreateVersion7(),
            AssessmentId = assessmentId,
            AssessmentCompleted = assessmentCompleted,
            AssessmentCreated = assessmentCreated,
            ParticipantId = participantId,
            TenantId = tenantId,
            EligibleForPayment = false,
            IneligibilityReason = ineligibilityReason.Value,
            PaymentPeriod = dates.Max(),
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            SupportWorker = supportWorker!
        };
    }

    public required Guid Id { get; set; }
    public required Guid AssessmentId { get; set; }
    public required DateTime AssessmentCompleted { get; set; }
    public required DateTime AssessmentCreated { get; set; }
    public required string ParticipantId { get; set; }
    public required string TenantId { get; set; }
    public required bool EligibleForPayment { get; set; }
    public required string? IneligibilityReason { get; set; }
    public required DateTime PaymentPeriod { get; set; }
    public required string ContractId { get; set; }
    public required int LocationId { get; set; }
    public required string LocationType { get; set; }
    public required string SupportWorker { get; set; }

}
