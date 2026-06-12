namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B9() : SingleChoiceQuestion("Felt loved and cared for at home",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B9);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};