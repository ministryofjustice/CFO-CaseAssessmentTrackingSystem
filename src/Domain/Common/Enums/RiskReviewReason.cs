using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class RiskReviewReason : SmartEnum<RiskReviewReason>
{
    public static readonly RiskReviewReason InitialReview = new("Initial review", 0, false, true);
    public static readonly RiskReviewReason ChangeToCircumstances = new("Change to circumstances", 1, false, true);
    public static readonly RiskReviewReason NoRiskInformationAvailable = new("No risk information available", 2, true, false);
    public static readonly RiskReviewReason NoChange = new("No change", 3, true, false);

    public bool RequiresJustification { get; }
    public bool RequiresFurtherInformation { get; }

    private RiskReviewReason(string name, int value, bool requiresJustification, bool requiresFurtherInformation) 
        : base(name, value) 
    {
        RequiresJustification = requiresJustification;
        RequiresFurtherInformation = requiresFurtherInformation;
    }

}
