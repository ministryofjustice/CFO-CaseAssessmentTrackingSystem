using System.ComponentModel;

namespace Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;

public class B2() : MultipleChoiceQuestion("Are you facing any of the following problems? (tick all that apply)",
[
    BehindOnRentOrMortgage,
    FacingEviction,
    LicenceRestrictionsForcedMove,
    DomesticIssuesForcedMove,
    RiskOfHomelessness,
    NoneOftheGivenOptions
])
{
    public const string BehindOnRentOrMortgage = "Behind on rent/mortgage";
    public const string FacingEviction = "Facing eviction";
    public const string LicenceRestrictionsForcedMove = "Having to move due to licence restrictions";
    public const string DomesticIssuesForcedMove = "Having to move due to domestic issues";
    public const string RiskOfHomelessness = "Worried may become homeless";
    public const string NoneOftheGivenOptions = "None of the above";
}