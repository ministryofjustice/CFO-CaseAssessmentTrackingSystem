using Ardalis.SmartEnum;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class Sex : SmartEnum<Sex>
{

    public static readonly Sex Male = new Sex(nameof(Male), 1);
    public static readonly Sex Female = new Sex(nameof(Female), 1);
    public static readonly Sex Unknown = new Sex(nameof(Unknown), 1);
    
    private Sex(string name, int value) : base(name, value)
    {
    }
}
