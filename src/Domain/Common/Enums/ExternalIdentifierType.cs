using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ExternalIdentifierType : SmartEnum<ExternalIdentifierType>
{
    public static readonly ExternalIdentifierType NomisNumber = new("NOMIS Number", 0, true);
    public static readonly ExternalIdentifierType Crn = new("Crn", 1, true);
    public static readonly ExternalIdentifierType PncNumber = new("PNC Number", 2, true);

    private ExternalIdentifierType(string name, int value, bool isExclusive = false) : base(name, value) 
    {
        IsExclusive = isExclusive;
    }

    /// <summary>
    /// Indicates whether the owner can have exactly one (exclusive), or multiple (inclusive) of each type of identifier.
    /// </summary>
    public bool IsExclusive { get; private set; }

}
