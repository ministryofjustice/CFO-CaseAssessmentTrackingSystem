using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public class ExternalIdentifierType : SmartEnum<ExternalIdentifierType>
{
    public static readonly ExternalIdentifierType NomisNumber = new(
        name: "NOMIS Number", 
        value: 0, 
        displayOrderPriority: 0, 
        isExclusive: true);

    public static readonly ExternalIdentifierType Crn = new(
        name: "Delius CRN", 
        value: 1, 
        displayOrderPriority: 1, 
        isExclusive: true);

    public static readonly ExternalIdentifierType PncNumber = new(
        name: "PNC Number", 
        value: 2, 
        displayOrderPriority: 2, 
        isExclusive: true);

    private ExternalIdentifierType(string name, int value, short displayOrderPriority, bool isExclusive = false) : base(name, value) 
    {
        IsExclusive = isExclusive;
        DisplayOrderPriority = displayOrderPriority;
    }

    /// <summary>
    /// Indicates whether the owner can have exactly one (exclusive), or multiple (inclusive) of each type of identifier.
    /// </summary>
    public bool IsExclusive { get; private set; }

    /// <summary>
    /// Determines the order in which identifiers are shown (lower is higher priority, appearing first).
    /// </summary>
    public short DisplayOrderPriority { get; private set; }
}
