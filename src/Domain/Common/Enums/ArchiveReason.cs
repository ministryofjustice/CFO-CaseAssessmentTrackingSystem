using Ardalis.SmartEnum;

namespace Cfo.Cats.Domain.Common.Enums;

public sealed class ArchiveReason : SmartEnum<ArchiveReason>
{
    public static readonly ArchiveReason None                       = new("Please select archive reason", -1, false);
    public static readonly ArchiveReason CaseloadTooHigh            = new("Caseload for this location is too high", 0, false);
    public static readonly ArchiveReason Deceased                   = new("Deceased", 1, false);
    public static readonly ArchiveReason LicenceEnd                 = new("Licence end date reached", 2, false);
    public static readonly ArchiveReason MovedOutsideProviderArea   = new("Moved outside of provider delivery catchment area", 3, false);
    public static readonly ArchiveReason MovedToNonCFO              = new("Moved to non-CFO location", 4, false);
    public static readonly ArchiveReason NoFurtherSupport           = new("No further support required", 5, false);
    public static readonly ArchiveReason NoRightToLiveWork          = new("No supporting right to live / work documentation", 6, false);
    public static readonly ArchiveReason NoWishToParticipate        = new("Participant has stated they do not wish to participate", 7, false);
    public static readonly ArchiveReason NoLongerEngaging           = new("Participant is no longer engaging with provision", 8, false);
    public static readonly ArchiveReason PersonalCircumstances      = new("Personal circumstances", 9, false);
    public static readonly ArchiveReason Other                      = new("Other", 10, true);

    public bool RequiresJustification { get; }
    
    private ArchiveReason(string name, int value, bool requiresJustification) 
        : base(name, value) 
    {
        RequiresJustification = requiresJustification;        
    }
}