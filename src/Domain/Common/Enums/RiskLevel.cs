using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class RiskLevel : SmartEnum<RiskLevel>
{
    public static readonly RiskLevel Low = new(nameof(Low), 1);
    public static readonly RiskLevel Medium = new(nameof(Medium), 2);
    public static readonly RiskLevel High = new(nameof(High), 3);
    public static readonly RiskLevel VeryHigh = new(nameof(VeryHigh), 4);

    public RiskLevel(string name, int value)
        : base(name, value) { }
}
