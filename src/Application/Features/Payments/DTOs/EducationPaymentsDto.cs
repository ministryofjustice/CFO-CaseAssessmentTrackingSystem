using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class EducationPaymentsDto
{
    public EducationPaymentDto[] Items { get; set; } = [];
    public List<EducationPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public class EducationPaymentDto
{
    public DateTime CreatedOn { get; set; }
    public DateTime CommencedOn { get; set; }
    public DateTime ActivityApproved { get; set; }

    public DateTime PaymentPeriod { get; set; }

    public string Contract { get; set; } = "";
    public string ParticipantId { get; set; } = "";
    public bool EligibleForPayment { get; set; }
    public string LocationType { get; set; } = "";
    public string Location { get; set; } = "";
    public string? IneligibilityReason { get; set; }
    public string ParticipantName { get; set; } = "";
    public string CourseLevel { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
}

public class EducationPaymentSummaryDto
{
    public required string Contract { get; set; }
    public required int Educations { get; set; }
    public required int EducationsTarget { get; set; }
    public decimal EducationsPercentage => Educations.CalculateCappedPercentage(EducationsTarget);
}
