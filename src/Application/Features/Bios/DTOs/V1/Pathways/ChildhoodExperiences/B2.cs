namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B2() : SingleChoiceQuestion("Spent most of childhood in the same family home",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B2);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};