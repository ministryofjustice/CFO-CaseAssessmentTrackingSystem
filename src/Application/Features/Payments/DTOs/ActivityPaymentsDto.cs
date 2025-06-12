using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class ActivityPaymentsDto
{
    public ActivityPaymentDto[] Items { get; set; } = [];
    public List<ActivityPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public class ActivityPaymentDto
{
    public DateTime CreatedOn { get; set; }
    public DateTime CommencedOn { get; set; }
    public DateTime ActivityApproved { get; set; }
    public DateTime PaymentPeriod { get; set; }
    public string Contract { get; set; } = "";
    public string ParticipantId { get; set; } = "";
    public bool EligibleForPayment { get; set; }
    public string Category { get; set; } = "";
    public string Type { get; set; } = "";
    public string LocationType { get; set; } = "";
    public string Location { get; set; } = "";
    public string? IneligibilityReason { get; set; }
    public string ParticipantName { get; set; } = "";
}

public class ActivityPaymentSummaryDto
{
    public required string Contract { get; set; }
    public int SupportWork { get; set; }
    public int SupportWorkTarget { get; set; }
    public decimal SupportWorkPercentage => SupportWork.CalculateCappedPercentage(SupportWorkTarget);
    public int CommunityAndSocial { get; set; }
    public int CommunityAndSocialTarget { get; set; }

    public decimal CommunityAndSocialPercentage =>
        CommunityAndSocial.CalculateCappedPercentage(CommunityAndSocialTarget);

    public int HumanCitizenship { get; set; }
    public int HumanCitizenshipTarget { get; set; }
    public decimal HumanCitizenshipPercentage => HumanCitizenship.CalculateCappedPercentage(HumanCitizenshipTarget);
    public int ISWSupport { get; set; }
    public int ISWSupportTarget { get; set; }
    public decimal ISWSupportPercentage => ISWSupport.CalculateCappedPercentage(ISWSupportTarget);
}