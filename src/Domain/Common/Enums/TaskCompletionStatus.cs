using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class TaskCompletionStatus : SmartEnum<TaskCompletionStatus>
{
    public static readonly TaskCompletionStatus Done = new("Done", 0);
    public static readonly TaskCompletionStatus Closed = new("Closed", 1);

    public TaskCompletionStatus(string name, int value)
        : base(name, value) { }
}
