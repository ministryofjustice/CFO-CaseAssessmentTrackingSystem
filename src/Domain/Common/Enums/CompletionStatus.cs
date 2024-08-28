using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class CompletionStatus : SmartEnum<CompletionStatus>
{
    public static readonly CompletionStatus Done = new("Done", 0, false);
    public static readonly CompletionStatus NotRequired = new("No longer required", 1, true);

    public CompletionStatus(string name, int value, bool requiresJustification = false)
        : base(name, value) 
    {
        RequiresJustification = requiresJustification;
    }

    public bool RequiresJustification { get; private set; }
}
