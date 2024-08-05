using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class RiskLevel : SmartEnum<RiskLevel>
{
    public static readonly RiskLevel Unknown = new(nameof(Unknown), -1);
    public static readonly RiskLevel Low = new("Low", 1);
    public static readonly RiskLevel Medium = new("Medium", 2);
    public static readonly RiskLevel High = new("High", 3);
    public static readonly RiskLevel VeryHigh = new("Very high", 4);

    public RiskLevel(string name, int value)
        : base(name, value) { }
}
