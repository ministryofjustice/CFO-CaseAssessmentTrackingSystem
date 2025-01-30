using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class PriAbandonReason : SmartEnum<PriAbandonReason>
{
    public static readonly PriAbandonReason CreatedByAccident = new("Created By Accident", 0, false);
    public static readonly PriAbandonReason Other = new("Other", 1, true);

    public bool RequiresJustification { get; }

    private PriAbandonReason(string name, int value, bool requiresJustification)
        : base(name, value)
    {
        RequiresJustification = requiresJustification;
    }
}