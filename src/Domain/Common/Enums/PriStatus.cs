using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class PriStatus : SmartEnum<PriStatus>
{
    public static readonly PriStatus Created = new(nameof(Created), 0, isActive: true);
    public static readonly PriStatus Accepted = new(nameof(Accepted), 1, isActive: true);
    public static readonly PriStatus Completed = new(nameof(Completed), 2, isActive: false);
    public static readonly PriStatus Abandoned = new(nameof(Abandoned), 3, isActive: false);

    private PriStatus(string name, int value, bool isActive = true)
        : base(name, value) 
    {
        IsActive = isActive;
    }

    public bool IsActive { get; private set; }

    public static IReadOnlyCollection<PriStatus> ActiveList => List.Where(p => p.IsActive).ToList();
}