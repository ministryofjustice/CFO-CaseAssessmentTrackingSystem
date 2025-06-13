using Ardalis.SmartEnum;
using Cfo.Cats.Application.Features.Payments.Extensions;

namespace Cfo.Cats.Application.Features.Payments.DTOs;

public class EnrolmentPaymentsDto
{
    public EnrolmentPaymentDto[] Items { get; set; } = [];
    public List<EnrolmentPaymentSummaryDto> ContractSummary { get; set; } = [];
}

public class EnrolmentPaymentDto
{
    public DateTime CreatedOn { get; set; }
    public DateTime SubmissionToAuthority { get; set; }
    public DateTime? Approved { get; set; }
    public string Contract { get; set; } = "";
    public string ParticipantId { get; set; } = "";
    public bool EligibleForPayment { get; set; }
    public string LocationType { get; set; } = "";
    public string Location { get; set; } = "";
    public string? IneligibilityReason { get; set; }
    public string ParticipantName { get; set; } = "";

    public bool IsCustody()
    {
        var type = SmartEnum<LocationType>.FromName(LocationType);
        return type.IsCustody;
    }
}

public class EnrolmentPaymentSummaryDto
{
    public required string Contract { get; set; }
    public int Custody { get; set; }

    public int CustodyTarget { get; set; }

    public decimal CustodyPercentage => Custody.CalculateCappedPercentage(CustodyTarget);

    public int Community { get; set; }

    public int CommunityTarget { get; set; }

    public decimal CommunityPercentage => Community.CalculateCappedPercentage(CommunityTarget);
}
