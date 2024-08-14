using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class TaskCompletionStatus : SmartEnum<TaskCompletionStatus>
{
    public static readonly TaskCompletionStatus Done = new("Done", 0, false);
    public static readonly TaskCompletionStatus NotRequired = new("No longer required", 1, true);

    public TaskCompletionStatus(string name, int value, bool requiresJustification = false)
        : base(name, value) 
    {
        RequiresJustification = requiresJustification;
    }

    public bool RequiresJustification { get; private set; }
}
