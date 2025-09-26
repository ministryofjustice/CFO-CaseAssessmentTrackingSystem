using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class ActivityAbandonReason : SmartEnum<ActivityAbandonReason>
{
    public static readonly ActivityAbandonReason NoLongerRequired = new("No longer required", 0, true);
    public static readonly ActivityAbandonReason NoLongerEngaged = new("No longer engaged", 1, true);
    public static readonly ActivityAbandonReason DuplicateClaim = new("Duplicate claim", 2, false);
    public static readonly ActivityAbandonReason Expired = new("Expired", 3, false);
    public static readonly ActivityAbandonReason CreatedByAccident = new("Created by accident", 4, false);
    public static readonly ActivityAbandonReason Other = new("Other", 5, true);

    public bool RequiresJustification { get; }

    private ActivityAbandonReason(string name, int value, bool requiresJustification)
        : base(name, value)
    {
        RequiresJustification = requiresJustification;
    }
}