using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class BioStatus : SmartEnum<BioStatus>
{
    public static readonly BioStatus NotStarted = new("Not started", 0);
    public static readonly BioStatus SkippedForNow = new("Skipped for now", 1);
    public static readonly BioStatus InProgress = new("In progress", 2);
    public static readonly BioStatus Complete = new("Complete", 3);

    private BioStatus(string name, int value)
        : base(name, value) { }
}
