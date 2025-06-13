using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class SupportAndReferralPaymentsDto
{
    public SupportAndReferralPaymentDto[] Items { get; set; } = [];
    public List<SupportAndReferralPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public record SupportAndReferralPaymentDto
{
    public required DateTime CreatedOn { get; set; }
    public required DateTime Approved { get; set; }
    public required string Contract { get; set; } = "";
    public required string ParticipantId { get; set; } = "";
    public required bool EligibleForPayment { get; set; }
    public required string SupportType { get; set; } = "";
    public required string LocationType { get; set; } = "";
    public required string Location { get; set; } = "";
    public required string? IneligibilityReason { get; set; }
    public required string ParticipantName { get; set; } = "";
    public required DateTime PaymentPeriod { get; set; }
}

public record SupportAndReferralPaymentSummaryDto
{
    public required string Contract { get; set; }
    public int PreReleaseSupport { get; set; }
    public int PreReleaseSupportTarget { get; set; }

    public decimal PreReleaseSupportPercentage =>
        PreReleaseSupport.CalculateCappedPercentage(PreReleaseSupportTarget);

    public int ThroughTheGateSupport { get; set; }
    public int ThroughTheGateSupportTarget { get; set; }

    public decimal ThroughTheGateSupportPercentage =>
        ThroughTheGateSupport.CalculateCappedPercentage(ThroughTheGateSupportTarget);
}