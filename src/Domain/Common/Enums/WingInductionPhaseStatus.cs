using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class WingInductionPhaseStatus : SmartEnum<WingInductionPhaseStatus>
{
    public static readonly WingInductionPhaseStatus Commenced = new(nameof(Commenced), 0, isActive: true);
    public static readonly WingInductionPhaseStatus Completed = new(nameof(Completed), 1, isActive: false);
    public static readonly WingInductionPhaseStatus Abandoned = new(nameof(Abandoned), 2, isActive: false);

    private WingInductionPhaseStatus(string name, int value, bool isActive = true)
        : base(name, value)
    {
        IsActive = isActive;
    }

    public bool IsActive { get; private set; }

    public static IReadOnlyCollection<WingInductionPhaseStatus> ActiveList => List.Where(p => p.IsActive).ToList();
}