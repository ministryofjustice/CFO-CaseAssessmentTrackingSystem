namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B12() : SingleChoiceQuestion("Parent or guardian had a drug / alcohol problem",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B12);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};