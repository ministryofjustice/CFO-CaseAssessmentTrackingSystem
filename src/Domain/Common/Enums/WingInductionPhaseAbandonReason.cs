using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class WingInductionPhaseAbandonReason : SmartEnum<WingInductionPhaseAbandonReason>
{
    public static readonly WingInductionPhaseAbandonReason Released = new("Released", 0, true);
    public static readonly WingInductionPhaseAbandonReason TransferredEstablishmentCatD = new("Transferred establishment - Cat D", 1, true);
    public static readonly WingInductionPhaseAbandonReason TransferredEstablishmentOther = new("Transferred establishment - Other", 2, true);
    public static readonly WingInductionPhaseAbandonReason ChangedMindDroppedOut = new("Changed mind/dropped out", 3, true);
    public static readonly WingInductionPhaseAbandonReason RemovedFromTheWingSafetySecurity = new ("Removed from the wing – safety/security", 4, true);
    public static readonly WingInductionPhaseAbandonReason HealthReasons = new("Health reasons", 5, true);
    public static readonly WingInductionPhaseAbandonReason PoorBehaviour = new("Poor behaviour", 6, true);
    public static readonly WingInductionPhaseAbandonReason OtherPriorities = new("Other priorities – e.g to complete an education or offending behaviour course", 7, true);
    public static readonly WingInductionPhaseAbandonReason Other= new("Other", 8, true);

    public bool RequiresJustification { get; }

    private WingInductionPhaseAbandonReason(string name, int value, bool requiresJustification)
        : base(name, value)
    {
        RequiresJustification = requiresJustification;
    }
}