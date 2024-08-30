using Cfo.Cats.Domain.Common.Enums;

namespace Cfo.Cats.Domain.Entities.Participants;

public class ExternalIdentifier : ValueObject
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ExternalIdentifier()
    {

    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ExternalIdentifier(string value, ExternalIdentifierType type)
    {
        Value = value;
        Type = type;
    }

    public static ExternalIdentifier Create(string value, ExternalIdentifierType type)
        => new (value, type);


    public string Value { get; private set; }
    public ExternalIdentifierType Type { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
        yield return Value;
    }

}
