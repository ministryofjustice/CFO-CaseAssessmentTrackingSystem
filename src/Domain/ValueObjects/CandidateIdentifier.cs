namespace Cfo.Cats.Domain.ValueObjects;

public class CandidateIdentifier : ValueObject
{
    public string IdentifierType { get; }
    public string IdentifierValue { get; }

    // required for EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CandidateIdentifier()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public CandidateIdentifier(string identifierType, string identifierValue)
    {
        if (string.IsNullOrEmpty(identifierType))
        {
            throw new ArgumentException("Identifier Type cannot be null or empty");
        }

        if (string.IsNullOrEmpty(identifierValue))
        {
            throw new ArgumentException("Identifier Value cannot be null or empty");
        }
        
        IdentifierType = identifierType;
        IdentifierValue = identifierValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IdentifierType;
        yield return IdentifierValue;
    }
}