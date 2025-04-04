using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class RiskDueReason : SmartEnum<RiskDueReason>
{
    public static readonly RiskDueReason NoRisk = new(nameof(NoRisk), 0);
    public static readonly RiskDueReason NewConsent = new(nameof(NewConsent), 1);
    public static readonly RiskDueReason DMSUpdated = new(nameof(DMSUpdated), 2);
    public static readonly RiskDueReason Reviewed = new(nameof(Reviewed), 3);
    public static readonly RiskDueReason Completed = new(nameof(Completed), 4);

    private RiskDueReason(string name, int value)
        : base(name, value) { }
}