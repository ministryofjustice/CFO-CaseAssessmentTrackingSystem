using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class GenderProvision : SmartEnum<GenderProvision>
{
    public static readonly GenderProvision Male = new(nameof(Male), 0);
    public static readonly GenderProvision Female = new(nameof(Female), 1);
    public static readonly GenderProvision Any = new(nameof(Any), 2);
    
    private GenderProvision(string name, int value)
        : base(name, value) { }
}
