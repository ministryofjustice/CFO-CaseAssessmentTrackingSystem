using Ardalis.SmartEnum;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class Sex : SmartEnum<Sex>
{
    public static readonly Sex Male = new(nameof(Male), 1);
    public static readonly Sex Female = new(nameof(Female), 1);
    public static readonly Sex Unknown = new(nameof(Unknown), 1);

    private Sex(string name, int value) : base(name, value)
    {
    }
}