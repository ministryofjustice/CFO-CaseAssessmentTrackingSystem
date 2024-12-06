using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ActivityStatus(string name, int value) : SmartEnum<ActivityStatus>(name, value)
{
    public static readonly ActivityStatus Submitted = new(nameof(Submitted), 0);
    public static readonly ActivityStatus SubmittedToProvider = new(nameof(SubmittedToProvider), 1);
    public static readonly ActivityStatus SubmittedToAuthority = new(nameof(SubmittedToAuthority), 2);
    public static readonly ActivityStatus ApprovedStatus = new(nameof(ApprovedStatus), 3);
}