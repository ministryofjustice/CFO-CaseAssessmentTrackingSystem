using Cfo.Cats.Domain.Entities.Assessments;

namespace Cfo.Cats.Domain.Entities.ManagementInformation;

public class ReassessmentPayment
{
#pragma warning disable CS8618
    private ReassessmentPayment() { }
#pragma warning restore CS8618

    public static ReassessmentPayment CreatePayable(ParticipantAssessment assessment, 
        string contractId,
        int locationId,
        string locationType)
    {
        var dates = new[]
        {
            assessment.Completed!.Value.Date,
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            // Enrolment approved?
        };

        return new()
        {
            Id = Guid.CreateVersion7(),
            AssessmentId = assessment.Id,
            AssessmentCompleted = assessment.Completed!.Value,
            AssessmentCreated = assessment.Created!.Value,
            ParticipantId = assessment.ParticipantId,
            TenantId = assessment.TenantId!,
            EligibleForPayment = true,
            IneligibilityReason = null,
            PaymentPeriod = dates.Max(),
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            SupportWorker = assessment.CompletedBy!
        };
    }

    public static ReassessmentPayment CreateNonPayable(ParticipantAssessment assessment, 
        string contractId,
        int locationId,
        string locationType,
        IneligibilityReason ineligibilityReason)
    {
        var dates = new[]
        {
            new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            assessment.Completed!.Value.Date
        };

        return new()
        {
            Id = Guid.CreateVersion7(),
            AssessmentId = assessment.Id,
            AssessmentCompleted = assessment.Completed!.Value,
            AssessmentCreated = assessment.Created!.Value,
            ParticipantId = assessment.ParticipantId,
            TenantId = assessment.TenantId!,
            EligibleForPayment = false,
            IneligibilityReason = ineligibilityReason.Value,
            PaymentPeriod = dates.Max(),
            ContractId = contractId,
            LocationId = locationId,
            LocationType = locationType,
            SupportWorker = assessment.CompletedBy!
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
