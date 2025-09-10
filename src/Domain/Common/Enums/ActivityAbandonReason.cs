using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class ActivityAbandonReason : SmartEnum<ActivityAbandonReason>
{
    public static readonly ActivityAbandonReason CreatedByAccident = new("Created By Accident", 0, false);
    public static readonly ActivityAbandonReason Other = new("Other", 1, true);

    public bool RequiresJustification { get; }

    private ActivityAbandonReason(string name, int value, bool requiresJustification)
        : base(name, value)
    {
        RequiresJustification = requiresJustification;
    }
}