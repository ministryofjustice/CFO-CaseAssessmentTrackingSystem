using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class RiskDueReason : SmartEnum<RiskDueReason>
{
    public static readonly RiskDueReason Unknown = new("No Reason Specified", 0);
    public static readonly RiskDueReason NewEntry = new("New Entry", 1);
    public static readonly RiskDueReason InitialReview = new("Initial Review", 2);
    public static readonly RiskDueReason DataFeedUpdated = new("Data Feed Updated", 3);
    public static readonly RiskDueReason TenWeekReview = new("Ten Week Review", 4);
    public static readonly RiskDueReason RemovedFromArchive = new("Removed From Archive", 5);
    
    private RiskDueReason(string name, int value)
        : base(name, value) { }
}