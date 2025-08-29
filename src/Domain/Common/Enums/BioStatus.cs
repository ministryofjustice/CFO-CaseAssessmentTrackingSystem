using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class BioStatus : SmartEnum<BioStatus>
{
    public static readonly BioStatus NotStarted = new(nameof(NotStarted), 0);
    public static readonly BioStatus SkippedForNow = new(nameof(SkippedForNow), 1);
    public static readonly BioStatus InProgress = new(nameof(InProgress), 2);
    public static readonly BioStatus Complete = new(nameof(Complete), 3);

    private BioStatus(string name, int value)
        : base(name, value) { }
}
