using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class InductionPaymentsDto
{
    public InductionPaymentDto[] Items { get; set; } = [];
    public List<InductionPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public class InductionPaymentDto
{
    public DateTime CreatedOn { get; set; }
    public DateTime CommencedOn { get; set; }
    public DateTime InductionDate { get; set; }
    public DateTime? Approved { get; set; }
    public DateTime? PaymentPeriod { get; set; }

    public string Contract { get; set; } = "";
    public string ParticipantId { get; set; } = "";
    public bool EligibleForPayment { get; set; }
    public string LocationType { get; set; } = "";
    public string Location { get; set; } = "";
    public string? IneligibilityReason { get; set; }
    public string ParticipantName { get; set; } = "";
}

public class InductionPaymentSummaryDto
{
    public required string Contract { get; set; }
    public int Wings { get; set; }
    public int WingsTarget { get; set; }
    public decimal WingsPercentage => Wings.CalculateCappedPercentage(WingsTarget);
    public int Hubs { get; set; }
    public int HubsTarget { get; set; }
    public decimal HubsPercentage => Hubs.CalculateCappedPercentage(HubsTarget);
}