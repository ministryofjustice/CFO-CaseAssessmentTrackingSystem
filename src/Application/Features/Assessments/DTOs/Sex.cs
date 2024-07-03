using Ardalis.SmartEnum;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class Sex : SmartEnum<Sex>
{
    public const string Male = "Male";
    public const string Female = "Female";
    public const string Unknown = "Unknown";
    
    public static readonly Sex MaleSex = new(Male, 1);
    public static readonly Sex FemaleSex = new(Female, 1);
    public static readonly Sex UnknownSex = new(Unknown, 1);

    private Sex(string name, int value) : base(name, value)
    {
    }
}