namespace Cfo.Cats.Application.Features.Bios.DTOs.V1.Pathways.ChildhoodExperiences;
public class B10() : SingleChoiceQuestion("Felt safe at home",
[
    Yes,
    No,
    NA
])
{
    public override string Code => nameof(B10);
    public const string Yes = "Yes";
    public const string No = "No";
    public const string NA = "PNTS";
};