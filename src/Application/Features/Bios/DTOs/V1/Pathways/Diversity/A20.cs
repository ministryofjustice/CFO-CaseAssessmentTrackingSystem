namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.Diversity;

public class A20() : SingleChoiceQuestion("Feel you are a better person than you used to be",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(A20);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};