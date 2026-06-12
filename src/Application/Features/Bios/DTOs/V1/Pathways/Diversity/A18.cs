namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A18() : SingleChoiceQuestion("Regret the offence(s) you committed",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(A18);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};