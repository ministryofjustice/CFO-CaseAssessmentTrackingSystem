using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class DipSampleStatus : SmartEnum<DipSampleStatus>
{
    private DipSampleStatus(string name, int value) : base(name, value)
    {
    }

    public static readonly DipSampleStatus AwaitingReview = new("Awaiting Review", 0);
    public static readonly DipSampleStatus Reviewed = new(nameof(Reviewed), 1);
    public static readonly DipSampleStatus Verifying = new(nameof(Verifying), 2);
    public static readonly DipSampleStatus Verified = new(nameof(Verified), 3);
    public static readonly DipSampleStatus Finalising = new(nameof(Finalising), 4);
    public static readonly DipSampleStatus Finalised = new(nameof(Finalised), 5);
}
