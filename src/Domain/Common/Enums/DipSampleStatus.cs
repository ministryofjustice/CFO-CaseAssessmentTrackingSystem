using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class DipSampleStatus : SmartEnum<DipSampleStatus>
{
    DipSampleStatus(string name, int value) : base(name, value) { }

    public static readonly DipSampleStatus InProgress = new(nameof(InProgress), 0);
    public static readonly DipSampleStatus Completed = new(nameof(Completed), 1);
}
