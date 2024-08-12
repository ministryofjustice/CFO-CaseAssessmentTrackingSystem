using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ReoccurrenceFrequency : SmartEnum<ReoccurrenceFrequency>
{
    public static readonly ReoccurrenceFrequency Daily = new("Daily", 0);
    public static readonly ReoccurrenceFrequency Weekly = new("Weekly", 1);
    public static readonly ReoccurrenceFrequency Monthly = new("Monthly", 2);
    public static readonly ReoccurrenceFrequency Yearly = new("Yearly", 3);

    private ReoccurrenceFrequency(string name, int value)
        : base(name, value) { }
}
