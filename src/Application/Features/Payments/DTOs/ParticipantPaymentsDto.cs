namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class ParticipantPaymentsDto
{
    public ParticipantPaymentDto[] Items { get; set; } = [];
}

public class ParticipantPaymentDto
{
    public required string PaymentType { get; set; }
    public string Description { get; set; } = "";
    public DateTime? PaymentPeriod { get; set; }
    public DateTime? Date { get; set; }
    public string Contract { get; set; } = "";
    public string Location { get; set; } = "";
    public string LocationType { get; set; } = "";
    public string SupportWorkerName { get; set; } = "";
    public bool EligibleForPayment { get; set; }
    public string? IneligibilityReason { get; set; }
}
