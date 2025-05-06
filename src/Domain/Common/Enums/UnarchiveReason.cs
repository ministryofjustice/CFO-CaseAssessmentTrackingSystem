using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class UnarchiveReason : SmartEnum<UnarchiveReason>
{
    public static readonly UnarchiveReason CaseloadManageable = new("Provider caseload now at a manageable level", 0, true);
    public static readonly UnarchiveReason NewSentence = new("Participant is serving a new sentence", 1, true);
    public static readonly UnarchiveReason MovedBackCatchmentArea = new("Participant moved back into provider catchment area", 2, true);
    public static readonly UnarchiveReason MovedCFOLocation = new("Participant moved back to CFO location", 3, true);
    public static readonly UnarchiveReason RequestedFurtherSupport = new("Participant requested further support", 4, true);
    public static readonly UnarchiveReason RTWDocumentatonAvailable = new("RTW documentation now available", 5, true);
    public static readonly UnarchiveReason ReestablishSupport = new("Participant would like to re-establish support", 6, true);
    public static readonly UnarchiveReason Reengaged = new("Participant has re-engaged", 7, true);
    public static readonly UnarchiveReason ChangePersonalCircumstances = new("Change in participants personal circumstances", 8, true);
    public static readonly UnarchiveReason ArchivedInError = new("Archived in error", 9, true);
    public static readonly UnarchiveReason Other = new("Other", 10, true);

    public bool RequiresFurtherInformation { get; }

    private UnarchiveReason(string name, int value, bool requiresFurtherInformation) 
        : base(name, value) 
    {
        RequiresFurtherInformation = requiresFurtherInformation;        
    }
}