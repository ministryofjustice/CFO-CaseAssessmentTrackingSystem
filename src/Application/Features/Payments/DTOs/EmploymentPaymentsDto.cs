using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class EmploymentPaymentsDto
{
    public EmploymentPaymentDto[] Items { get; set; } = [];
    public List<EmploymentPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public class EmploymentPaymentDto
{
    public required DateTime CreatedOn { get; set; }
    public required DateTime CommencedOn { get; set; }
    public required DateTime ActivityApproved { get; set; }
    public required DateTime PaymentPeriod { get; set; }
    public required string Contract { get; set; }
    public required string LocationType { get; set; }
    public required string Location { get; set; }
    public required string ParticipantId { get; set; }
    public required bool EligibleForPayment { get; set; }
    public required string? IneligibilityReason { get; set; }
    public required string ParticipantName { get; set; }
    public required string EmploymentType { get; set; }
    public required string EmploymentCategory { get; set; }
}

public class EmploymentPaymentSummaryDto
{
    public required string Contract { get; set; }
    public required int Employments { get; set; }
    public required int EmploymentsTarget { get; set; }
    public decimal EmploymentsPercentage => Employments.CalculateCappedPercentage(EmploymentsTarget);
}
