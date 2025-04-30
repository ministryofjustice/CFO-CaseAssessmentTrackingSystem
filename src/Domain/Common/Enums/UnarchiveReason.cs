using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class UnarchiveReason : SmartEnum<UnarchiveReason>
{
    public static readonly UnarchiveReason ReengagedAfterAbsence      = new("Re-engaged following a period of absence", 0, false);
    public static readonly UnarchiveReason MovedToNonCFO              = new("Was moved to a non-CFO (custodial) location", 1, false);
    public static readonly UnarchiveReason UnableEngageCFOCommunity   = new("Was not able to engage with CFO community provision", 2, false);
    public static readonly UnarchiveReason InCustody                  = new("New offence / stint in custody", 3, false);
    public static readonly UnarchiveReason RelationshipIssues         = new("Relationship issues", 4, false);
    public static readonly UnarchiveReason Unemployed                 = new("Unemployed", 5, false);
    public static readonly UnarchiveReason ArchivedByAccident         = new("Archived by accident", 6, false);
    public static readonly UnarchiveReason Other                      = new("Other", 7, true);

    public bool RequiresFurtherInformation { get; }

    private UnarchiveReason(string name, int value, bool requiresFurtherInformation) 
        : base(name, value) 
    {
        RequiresFurtherInformation = requiresFurtherInformation;        
    }
}