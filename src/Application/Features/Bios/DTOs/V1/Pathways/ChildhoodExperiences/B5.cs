namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B5() : SingleChoiceQuestion("Both parents/guardians were mostly present",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B5);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};