using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class RiskDueReason : SmartEnum<RiskDueReason>
{
    public static readonly RiskDueReason NoRisk = new("No Risk", 0);
    public static readonly RiskDueReason NewConsent = new("New Consent", 1);
    public static readonly RiskDueReason DMSUpdated = new("Update From DMS Feed", 2);
    public static readonly RiskDueReason Reviewed = new("Risk Reviewed", 3);
    public static readonly RiskDueReason Completed = new("Completed", 4);
        
    private RiskDueReason(string name, int value)
        : base(name, value) { }
}